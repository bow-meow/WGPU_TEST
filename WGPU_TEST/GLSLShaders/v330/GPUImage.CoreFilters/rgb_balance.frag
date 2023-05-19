#version 330 core

uniform sampler2D input_texture;

uniform float red;
uniform float green;
uniform float blue;

//in VertexData{
//	vec2 uv;
//	vec2 uv2;
//} vert_out;

in vec2 v_tex;

out vec4 frag_colour;

void main()
{
	vec4 src = texture(input_texture, v_tex);

	src.r += red*2;
	src.g += green*2;
	src.b += blue*2;
	frag_colour = clamp(src, 0, 1);
}