#version 330 core

out vec4 FragColor;
in vec2 fTexCoords;
in vec2 fUv;
uniform sampler2D uTexture0;

// Function to generate Perlin noise
float noise(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453123);
}

// Function to interpolate between values
float smoothNoise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(mix(noise(i + vec2(0.0, 0.0)), noise(i + vec2(1.0, 0.0)), u.x),
               mix(noise(i + vec2(0.0, 1.0)), noise(i + vec2(1.0, 1.0)), u.x), u.y);
}

// Function to generate fractal noise
float fractalNoise(vec2 p) {
    float value = 0.0;
    float amplitude = 0.5;
    for (int i = 0; i < 6; i++) {
        value += amplitude * smoothNoise(p);
        p *= 2.0;
        amplitude *= 0.5;
    }
    return value;
}

void main() {
    vec2 uv = fUv * 5.0; // Scale UV coordinates for cloud effect
    float cloud = fractalNoise(uv);
    vec4 Sacrificalvalue = texture(uTexture0,fUv); //A stub to get the engine to not complain about not finding uTexture0
    FragColor = vec4(vec3(cloud), 1.0); // Output cloud color
}
