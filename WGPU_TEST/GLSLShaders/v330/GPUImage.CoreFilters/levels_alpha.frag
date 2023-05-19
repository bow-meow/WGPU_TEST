#version 430 core

uniform sampler2D input_texture;

uniform float inBlack;
uniform float inWhite;
uniform float inGamma;
uniform float outWhite;
uniform float outBlack;


in VertexData{
	vec2 uv;
	vec2 uv2;
} vert_out;

out vec4 frag_colour;

vec4 Levels(vec4 color)
{
	vec4 one = vec4(1.0, 1.0f, 1.0f, 1.0f);
	vec4 zero = vec4(0.0, 0.0f, 0.0f, 0.0f);
	vec4 i = min(one, (max(color - inBlack, zero) / (inWhite-inBlack) ) );
	vec4 p = vec4(pow(i.x, inGamma), pow(i.y, inGamma), pow(i.z, inGamma), pow(i.w, inGamma));
	p = (p * (outWhite-outBlack)) + outBlack;
	return p;
}

void main()
{
	vec4 src = texture(input_texture, vert_out.uv);

	vec4 s = Levels(src.wwww);

	frag_colour = vec4(src.xyz, s.x);
}