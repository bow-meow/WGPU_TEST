#version 430 core

uniform sampler2D input_texture;

uniform vec4 QuadScreenSize;
uniform float QuadTexOffset;

in VertexData{
	vec2 uv;
	vec2 uv2;
} vert_out;

out vec4 frag_colour;

vec4 convert(float y, float v, float u)
{
    float rr = clamp(y + 1.402 * (u - 0.5), 0, 1);
    float gg = clamp(y - 0.344 * (v - 0.5) - 0.714 * (u - 0.5), 0, 1);
    float bb = clamp(y + 1.772 * (v - 0.5), 0, 1);
    return vec4(rr, gg, bb, 1);
}

void main()
{
    vec2 uv = vert_out.uv;
	uv.x /= 2;
    vec4 col = texture(input_texture, uv);

    float realTexCoordX = uv.x * QuadScreenSize.x;

    float xoffset = (QuadTexOffset * 1.0) / QuadScreenSize.x;
    if (fract(realTexCoordX) >= 0.5)
    {
        frag_colour = convert(col.x, col.w, col.y);
        return;
    }

    vec4 col2 = texture(input_texture, uv - vec2(xoffset, 0));
    vec4 col3 = texture(input_texture, uv + vec2(xoffset, 0));

    float u = (col2.y + col3.y) / 2.0;
    float v = (col2.w + col3.w) / 2.0;
    frag_colour = convert(col.z, v, u);
}