#version 430 core

uniform sampler2D input_texture;

in VertexData{
	vec2 uv;
	vec2 uv2;
} vert_out;

out vec4 frag_colour;

float PerceivedBrightness(float r, float g, float b)
{
	return sqrt(r * r* 0.241 + g * g * 0.691 + b * b * 0.068);
}

void main()
{
	vec4 src = texture(input_texture, vert_out.uv);
	float p = PerceivedBrightness(src.r, src.g, src.b);
	frag_colour = vec4(p, p, p, 1.0);
}