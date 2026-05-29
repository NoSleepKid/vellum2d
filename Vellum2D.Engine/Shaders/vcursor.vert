#version 430 core
layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec2 aTexCoords;

out vec2 TexCoords;
uniform mat4 uProjection;

void main() {
    TexCoords = aTexCoords;
    gl_Position = vec4(aPosition, 0.0, 1.0);
}
