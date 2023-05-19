struct VertexOutput {
    @builtin(position) clip_position: vec4<f32>,
    @location(0) tex_coords: vec2<f32>,
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

@group(1) @binding(0) var<uniform> data: DataUniform;

struct DataUniform{
    quad_screen_size: vec4<f32>
};

fn convertHDYC(y: f32, v: f32, u: f32) -> vec4<f32>
{
    let rr = clamp(1.164 * (y - (16.0 / 255.0)) + 1.793 * (u - 0.5), 0.0, 1.0);
    let gg = clamp(1.164 * (y - (16.0 / 255.0)) - 0.534 * (u - 0.5) - 0.213 * (v - 0.5), 0.0, 1.0);
    let bb = clamp(1.164 * (y - (16.0 / 255.0)) + 2.115 * (v - 0.5), 0.0, 1.0);
    return vec4<f32>(rr, gg, bb, 1.0);
}

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>
{
    var uv: vec2<f32> = in.tex_coords;
    uv.x /= 2.0;
    var src: vec4<f32> = textureSample(input_tex, input_sampler, uv);

    let real_tex_coord_x = uv.x * data.quad_screen_size.x;

    if (fract(real_tex_coord_x) < 0.5){
        src = convertHDYC(src.y, src.z, src.x);
    }
    else{
        src = convertHDYC(src.w, src.z, src.x);
    }
    return src;
}