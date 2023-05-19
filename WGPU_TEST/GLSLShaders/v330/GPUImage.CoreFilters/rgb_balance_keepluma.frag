#version 430 core

uniform sampler2D input_texture;

uniform float red;
uniform float green;
uniform float blue;

in VertexData{
	vec2 uv;
	vec2 uv2;
} vert_out;

out vec4 frag_colour;

vec4 rgb2yuv(vec4 rgb)
{
	mat4 coeffs = mat4(
			 0.299, 0.587, 0.114, 0.000,
			-0.147,-0.289, 0.436, 0.000,
			 0.615,-0.515,-0.100, 0.000,
			 0.000, 0.000, 0.000, 0.000
		);
		
	return coeffs * rgb;
}

vec4 yuv2rgb(vec4 yuv)
{
	mat4 coeffs = mat4(
			 1.000, 0.000, 1.140, 0.000,
			 1.000,-0.395,-0.581, 0.000,
			 1.000, 2.032, 0.000, 0.000,
			 0.000, 0.000, 0.000, 0.000
		);
	
	return coeffs * yuv;
}

void main()
{
	vec4 src = texture(input_texture, vert_out.uv);
	vec4 yuv = rgb2yuv(src);

	src.r += red*2;
	src.g += green*2;
	src.b += blue*2;
	src = clamp(src, 0, 1);

	vec4 yuv2 = rgb2yuv(src);
	src.xyz = yuv2rgb(vec4(yuv.x, yuv2.yz, 1)).xyz;

	frag_colour = src;
}