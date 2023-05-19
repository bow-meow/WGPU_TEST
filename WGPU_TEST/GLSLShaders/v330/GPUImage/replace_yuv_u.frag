#version 430 core

uniform sampler2D input_texture;

//in VertexData {
//	vec2 uv;
//	vec2 uv2;
//} vert_out;

in vec2 v_tex;

out vec4 frag_colour;

vec4 RGB2YUV(vec4 rgb)
{
	mat4 coeffs = mat4(
			 0.299, 0.587, 0.114, 0.000,
			-0.147,-0.289, 0.436, 0.000,
			 0.615,-0.515,-0.100, 0.000,
			 0.000, 0.000, 0.000, 0.000
		);
		
	return coeffs * rgb;
}

void main()
{
	vec4 src = texture(input_texture, v_tex);
    vec4 yuv = RGB2YUV(src);
    
	frag_colour = vec4(yuv.yyy + 0.5, 1.0);
}