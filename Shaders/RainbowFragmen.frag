#version 330 core

in vec2 fUv;
uniform sampler2D uTexture0;
uniform float time;

out vec4 FragColor;

float PHI = 1.61803398874989484820459;

float gold_noise(in vec2 xy, in float seed){
    return fract(tan(distance(xy*PHI,xy)*seed)*xy.x);
}

void main(){
    vec4 Sacrificalvalue = texture(uTexture0,fUv);
    FragColor = vec4(vec3(gold_noise(fUv,time)),1.0);
}