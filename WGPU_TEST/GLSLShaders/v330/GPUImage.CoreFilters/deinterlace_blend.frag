#version 430 core

uniform sampler2D input_texture;

uniform vec4 pixelSize;

in VertexData{
	vec2 uv;
	vec2 uv2;
} vert_out;

out vec4 frag_colour;

void main()
{
	vec4 col = texture(input_texture, vert_out.uv);
	
	vec2 h = vec2(0, pixelSize.y*2);
	vec4 c1 = texture(input_texture, vert_out.uv-h);
	vec4 c2 = texture(input_texture, vert_out.uv+h);
	frag_colour = (col*2 +c1 + c2)/4;
}