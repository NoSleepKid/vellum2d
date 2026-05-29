#version 330 core
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

void main() {
    // Distinct base background color to prove the fragment shader execution pipeline is active
    vec4 finalColor = vec4(0.2, 0.3, 0.4, 1.0); 
    int highestLayer = -1;
    
    for(int i = 0; i < uSpriteCount; i++) {
        SpriteData sprite = uSprites[i];
        if(TexCoords.x >= sprite.posX && TexCoords.x <= (sprite.posX + sprite.sizeX) &&
           TexCoords.y >= sprite.posY && TexCoords.y <= (sprite.posY + sprite.sizeY)) {
            if(sprite.layer > highestLayer) {
                highestLayer = sprite.layer;
                finalColor = vec4(0.85, 0.35, 0.35, 1.0); 
            }
        }
    }
    FragColor = finalColor;
}
