#version 330 core

uniform sampler2D input_texture;

float whitetolerance;
uniform float tolerance;
uniform vec4 newblack;
uniform vec4 newwhite;

//in VertexData{
//	vec2 uv;
//	vec2 uv2;
//} vert_out;

in vec2 v_tex;

out vec4 frag_colour;

vec4 RGB2HighlightRGB(vec4 color)
{
	vec4 result = color;
	if(color.r <= tolerance && color.g <= tolerance && color.b <= tolerance)
		{
			result = newblack;
			result.a = color.a;
		}
	
	if(color.r >= whitetolerance && color.g >= whitetolerance && color.b >= whitetolerance)
		{
			result = newwhite;
			result.a = color.a;
		}
	
	return result;
}

void main()
{
	whitetolerance = 1.0 - tolerance;
	frag_colour = RGB2HighlightRGB(texture(input_texture, v_tex));
}
