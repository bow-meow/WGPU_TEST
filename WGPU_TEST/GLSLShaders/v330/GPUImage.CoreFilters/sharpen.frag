#version 330 core

uniform sampler2D input_texture;

uniform vec4 pixelSize;
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
	vec4 c = c0 * 13;

	c += texture(input_texture, v_tex + vec2(pixelSize.x, 0)) * -2;
	c += texture(input_texture, v_tex + vec2(-pixelSize.x, 0)) * -2;
	
	c += texture(input_texture, v_tex + vec2(pixelSize.x, -pixelSize.y)) * -1;
	c += texture(input_texture, v_tex + vec2(0, -pixelSize.y)) * -2;
	c += texture(input_texture, v_tex + vec2(-pixelSize.x, -pixelSize.y)) * -1;

	c += texture(input_texture, v_tex + vec2(pixelSize.x, pixelSize.y)) * -1;
	c += texture(input_texture, v_tex + vec2(0, pixelSize.y)) * -2;
	c += texture(input_texture, v_tex + vec2(-pixelSize.x, pixelSize.y)) * -1;
	
	frag_colour = mix(c0, c, factor);
}