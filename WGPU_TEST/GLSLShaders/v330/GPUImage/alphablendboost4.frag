#version 120

uniform sampler2D input_texture;

in VertexData{
	vec2 uv;
	vec2 uv2;
} vert_out;

out vec4 frag_colour;

void main()
{
	vec4 src = texture(input_texture, vert_out.uv);
	frag_colour = vec4(1, 1, 1, 1);

	if (src.a <= 0.0)
		frag_colour *= 0;
	else
		if (src.a < 0.5)
			frag_colour *= 0.3333;
		else if (src.a < 1.0)
			frag_colour *= 0.6666;

	else
		frag_colour *= 1;

	frag_colour.a = 1;
}