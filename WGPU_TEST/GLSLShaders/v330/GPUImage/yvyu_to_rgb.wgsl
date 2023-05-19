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

fn convert(y: f32, v: f32, u: f32) -> vec4<f32>
{
    let rr: f32 = clamp(y + 1.402 * (u - 0.5), 0.0, 1.0);
    let gg: f32 = clamp(y - 0.344 * (v - 0.5) - 0.714 * (u - 0.5), 0.0, 1.0);
    let bb: f32 = clamp(y + 1.772 * (v - 0.5), 0.0, 1.0);
    return vec4<f32>(rr, gg, bb, 1.0);
}

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>{
    var uv: vec2<f32> = in.tex_coords;
	uv.x /= 2.0;
    let src: vec4<f32> = textureSample(input_tex, input_sampler, uv);

    let real_texcoord_x: f32 = uv.x * data.quad_screen_size.x;

    if (fract(real_texcoord_x) > 0.5){
        return convert(src.x, src.w, src.y);
    }
    return convert(src.z, src.w, src.y);
}