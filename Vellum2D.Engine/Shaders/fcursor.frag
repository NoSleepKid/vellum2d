#version 430 core
out vec4 FragColor;
in vec2 TexCoords;

struct SpriteData {
    float posX;
    float posY;
    float sizeX;
    float sizeY;
    int layer;
    int textureID;
};

layout (std430, binding = 0) buffer SpriteBuffer {
    SpriteData uSprites[];
};

uniform int uSpriteCount;
uniform vec2 uResolution;

void main() {
    // Correct texcoords into absolute pixel measurements to normalize aspect-ratio shifts
    vec2 fragPixel = TexCoords * uResolution;
    
    vec4 finalColor = vec4(0.13, 0.17, 0.22, 1.0); 
    int highestLayer = -1;
    
    for(int i = 0; i < uSpriteCount; i++) {
        SpriteData sprite = uSprites[i];
        
        // Convert normalized positions into absolute window pixel layouts dynamically
        vec2 spritePixelPos = vec2(sprite.posX, sprite.posY) * uResolution;
        vec2 spritePixelSize = vec2(sprite.sizeX, sprite.sizeY) * uResolution;

        if(fragPixel.x >= spritePixelPos.x && fragPixel.x <= (spritePixelPos.x + spritePixelSize.x) &&
           fragPixel.y >= spritePixelPos.y && fragPixel.y <= (spritePixelPos.y + spritePixelSize.y)) {
            if(sprite.layer > highestLayer) {
                highestLayer = sprite.layer;
                finalColor = vec4(0.85, 0.35, 0.35, 1.0); 
            }
        }
    }
    FragColor = finalColor;
}
