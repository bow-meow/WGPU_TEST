#version 430 core

in vec2 position;
in vec2 uv;

uniform vec2 bgTileOffset;
uniform vec2 bgTileScale;

out VertexData2{
	vec4 uv;
} vert_out2;

void main()
{
	vec2 sgn=sign(position.xy);
    
    // uv
	vert_out2.uv.xy = uv;
	vert_out2.uv.zw = bgTileOffset + (uv * bgTileScale);

    gl_Position = vec4(sgn, 0, 1);
}