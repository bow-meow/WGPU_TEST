#version 430 core

uniform sampler2D input_texture;

uniform float contrast;
uniform float brightness;

in VertexData{
	vec2 uv;
	vec2 uv2;
} vert_out;

out vec4 frag_colour;

void main()
{
	vec4 src = texture(input_texture, vert_out.uv);
	frag_colour = vec4(((src.rgb - 0.5) * contrast) + 0.5 + brightness, src.a);
}