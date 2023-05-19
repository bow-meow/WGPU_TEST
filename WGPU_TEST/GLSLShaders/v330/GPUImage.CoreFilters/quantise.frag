#version 330 core

uniform sampler2D input_texture;
uniform float factor;

//in VertexData{
//	vec2 uv;
//	vec2 uv2;
//} vert_out;

in vec2 v_tex;

out vec4 frag_colour;

void main()
{
	vec4 c0 = texture(input_texture, v_tex);
	
	c0.r = (int(c0.r * factor) / factor);
	c0.g = (int(c0.g * factor) / factor);
	c0.b = (int(c0.b * factor) / factor);
	
	frag_colour = c0;
}