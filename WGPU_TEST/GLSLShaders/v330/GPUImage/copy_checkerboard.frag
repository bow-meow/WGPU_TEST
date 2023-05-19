#version 330 core

uniform sampler2D input_texture;
uniform vec2 windowSize;

//in VertexData {
//	vec2 uv;
//	vec2 uv2;
//} vert_out;

in vec2 v_tex;

out vec4 frag_colour;

void main()
{
	vec4 col = texture(input_texture, v_tex);
	vec4 checkerboard = vec4(0.5, 0.5, 0.5, 1.0);
	if((int(floor(v_tex.x * windowSize.x) + floor(v_tex.y * windowSize.y)) & 1) == 0)
			checkerboard = vec4(1.0, 1.0, 1.0, 1.0);
//	frag_colour = checkerboard;
 	frag_colour = vec4(col.rgb * col.a + (1.0 - col.a) * checkerboard.rgb, 1.0);
}
