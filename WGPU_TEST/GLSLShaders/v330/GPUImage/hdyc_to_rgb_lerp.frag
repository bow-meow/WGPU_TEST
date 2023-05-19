#version 430 core

uniform sampler2D input_texture;

uniform vec4 QuadScreenSize;

in VertexData{
	vec2 uv;
	vec2 uv2;
} vert_out;

out vec4 frag_colour;

vec4 convertHDYC(float y, float v, float u)
{
    float rr = clamp(1.164 * (y - (16.0 / 255.0)) + 1.793 * (u - 0.5), 0, 1);
    float gg = clamp(1.164 * (y - (16.0 / 255.0)) - 0.534 * (u - 0.5) - 0.213 * (v - 0.5), 0, 1);
    float bb = clamp(1.164 * (y - (16.0 / 255.0)) + 2.115 * (v - 0.5), 0, 1);
    return vec4(rr, gg, bb, 1);
}


void main()
{
    vec2 uv = vert_out.uv;
    uv.x /= 2;
    vec4 col = texture(input_texture, uv);

    float realTexCoordX = uv.x * QuadScreenSize.x;

    if (fract(realTexCoordX) < 0.5)
    {
        frag_colour = convertHDYC(col.y, col.z, col.x);
    }
    else
    {
        float xoffset = 1.0 / QuadScreenSize.x;
        vec4 col2 = texture(input_texture, uv + vec2(xoffset, 0));

        float u = (col.z + col2.z) * 0.5;
        float v = (col.x + col2.x) * 0.5;
        frag_colour = convertHDYC(col.w, u, v);
    }
}