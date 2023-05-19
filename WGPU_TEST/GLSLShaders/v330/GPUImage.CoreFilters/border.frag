#version 330 core

uniform sampler2D input_texture;

uniform vec4 colour;
uniform vec4 colourMask;
uniform float top;
uniform float left;
uniform float bottom;
uniform float right;

in vec2 v_tex;

out vec4 frag_colour;

void main()
{
	vec4 src = texture(input_texture, v_tex);
	frag_colour = src;
	if (v_tex.y < top || v_tex.y > bottom || v_tex.x < left || v_tex.x > right)
		frag_colour = colour;
}