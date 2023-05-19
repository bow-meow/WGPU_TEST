#version 430 core

uniform sampler2D input_texture;

uniform vec4 pixelSize;
uniform float radius;

in VertexData{
	vec2 uv;
	vec2 uv2;
} vert_out;

out vec4 frag_colour;

vec4 Blur(vec2 uv, vec2 offset)
{
	vec4 result;
	vec4 col = texture(input_texture, uv);
	vec4 c = col;

	for (int i=1; i<=radius; i++)
		c += texture(input_texture, uv - offset*i);

	for (int i=1; i<=radius; i++)
		c += texture(input_texture, uv + offset*i);

	c /= (1.0 + radius + radius);
	result = c;

	return result;
}

void main()
{
	frag_colour = Blur(vert_out.uv, vec2(0.0, pixelSize.y*1.5));
}