#version 430 core

in vec2 position;
in vec2 uv;
in vec2 uv2;

out VertexData{
	vec2 uv;
	vec2 uv2;
} vert_out;

void main()
{
	vec2 sgn = sign(position.xy);
	vert_out.uv = uv;
	vert_out.uv2 = uv2;

	gl_Position = vec4(sgn,0,1);
}