#version 430 core

uniform sampler2D input_texture;
uniform sampler2D mask_texture;

in VertexData{
	vec2 uv;
	vec2 uv2;
} vert_out;

out vec4 frag_colour;

void main()
{
	vec4 fg = texture(input_texture, vert_out.uv);
	float mask = texture(mask_texture, vert_out.uv).a;
	frag_colour = vec4(fg.xyz, fg.a - mask);
}