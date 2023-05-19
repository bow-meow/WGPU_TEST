#version 330 core

uniform sampler2D input_texture;

//in VertexData {
//	vec2 uv;
//	vec2 uv2;
//} vert_out;

in vec2 v_tex;

out vec4 frag_colour;

void main()
{
	frag_colour = texture(input_texture, v_tex);
}