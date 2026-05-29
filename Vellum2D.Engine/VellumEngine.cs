using Silk.NET.Windowing;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace Vellum2D.Engine;

public class VellumEngine {
    private IWindow _window = null!;
    private GL _gl = null!;
    private IInputContext _input = null!;
    private uint _vao, _vbo, _ssbo, _shaderProgram;
    private GameScene? _activeScene;
    private GpuSpriteData[] _gpuBufferCpuSide = new GpuSpriteData[100];

    public void SetScene(GameScene scene) {
        _activeScene = scene;
        _activeScene.OnInitialize();
    }

    public void Run(int width = 1280, int height = 720, string title = "Vellum2D Engine") {
        var options = WindowOptions.Default;
        options.Size = new Silk.NET.Maths.Vector2D<int>(width, height);
        options.Title = title;
        options.VSync = true;
        _window = Window.Create(options);
        _window.Load += OnLoad;
        _window.Update += OnUpdate;
        _window.Render += OnRender;
        _window.Closing += OnClosing;
        _window.Run();
    }

    private unsafe void OnLoad() {
        _gl = GL.GetApi(_window);
        _input = _window.CreateInput();
        foreach (var keyboard in _input.Keyboards) {
            keyboard.KeyDown += (kb, key, code) => { if (key == Key.Escape) _window.Close(); };
        }
        float[] quadVertices = {
            -1.0f,  1.0f,  0.0f, 1.0f,
            -1.0f, -1.0f,  0.0f, 0.0f,
             1.0f, -1.0f,  1.0f, 0.0f,

            -1.0f,  1.0f,  0.0f, 1.0f,
             1.0f, -1.0f,  1.0f, 0.0f,
             1.0f,  1.0f,  1.0f, 1.0f
        };
        _vao = _gl.GenVertexArray(); _vbo = _gl.GenBuffer();
        _gl.BindVertexArray(_vao); _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
        fixed (void* v = quadVertices) {
            _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(quadVertices.Length * sizeof(float)), v, BufferUsageARB.StaticDraw);
        }
        _gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), (void*)0);
        _gl.EnableVertexAttribArray(0);
        _gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), (void*)(2 * sizeof(float)));
        _gl.EnableVertexAttribArray(1);

        // Instantiating a modern Shader Storage Buffer Object (SSBO) layout pipeline
        _ssbo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ShaderStorageBuffer, _ssbo);
        _gl.BufferData(BufferTargetARB.ShaderStorageBuffer, (nuint)(100 * sizeof(GpuSpriteData)), null, BufferUsageARB.DynamicDraw);
        _gl.BindBufferBase(BufferTargetARB.ShaderStorageBuffer, 0, _ssbo);

        CompileCursorShaders();
    }

    private void CompileCursorShaders() {
        string basePath = Directory.Exists("Shaders") ? "Shaders" : "Vellum2D.Engine/Shaders";
        string vertCode = File.ReadAllText(Path.Combine(basePath, "vcursor.vert"));
        string fragCode = File.ReadAllText(Path.Combine(basePath, "fcursor.frag"));
        uint vertexShader = _gl.CreateShader(ShaderType.VertexShader); _gl.ShaderSource(vertexShader, vertCode); _gl.CompileShader(vertexShader);
        uint fragmentShader = _gl.CreateShader(ShaderType.FragmentShader); _gl.ShaderSource(fragmentShader, fragCode); _gl.CompileShader(fragmentShader);
        _shaderProgram = _gl.CreateProgram(); _gl.AttachShader(_shaderProgram, vertexShader); _gl.AttachShader(_shaderProgram, fragmentShader); _gl.LinkProgram(_shaderProgram);
        _gl.DeleteShader(vertexShader); _gl.DeleteShader(fragmentShader);
    }

    private void OnUpdate(double deltaTime) => _activeScene?.OnUpdate(deltaTime);

    private unsafe void OnRender(double deltaTime) {
        _gl.Clear((uint)ClearBufferMask.ColorBufferBit);
        _gl.UseProgram(_shaderProgram);

        if (_activeScene != null) {
            var sprites = _activeScene.Sprites;
            int count = Math.Min(sprites.Count, 100);
            _gl.Uniform1(_gl.GetUniformLocation(_shaderProgram, "uSpriteCount"), count);

            for (int i = 0; i < count; i++) {
                _gpuBufferCpuSide[i] = sprites[i].ToGpuData();
            }

            _gl.BindBuffer(BufferTargetARB.ShaderStorageBuffer, _ssbo);
            
            // Re-allocate structural block target size to isolate GPU execution states cleanly
            _gl.BufferData(BufferTargetARB.ShaderStorageBuffer, (nuint)(100 * sizeof(GpuSpriteData)), null, BufferUsageARB.DynamicDraw);
            
            fixed (void* ptr = _gpuBufferCpuSide) {
                _gl.BufferSubData(BufferTargetARB.ShaderStorageBuffer, 0, (nuint)(count * sizeof(GpuSpriteData)), ptr);
            }
        }

        _gl.BindVertexArray(_vao); 
        _gl.DrawArrays(PrimitiveType.Triangles, 0, 6);
        _window.SwapBuffers();
    }

    private void OnClosing() { _gl.DeleteVertexArray(_vao); _gl.DeleteBuffer(_vbo); _gl.DeleteBuffer(_ssbo); _gl.DeleteProgram(_shaderProgram); _input.Dispose(); _gl.Dispose(); }
}
