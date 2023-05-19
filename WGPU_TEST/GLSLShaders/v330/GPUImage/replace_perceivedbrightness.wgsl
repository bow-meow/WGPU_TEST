struct VertexOutput {
    @builtin(position) clip_position: vec4<f32>,
    @location(0) tex_coords: vec2<f32>,
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

fn PerceivedBrightness(r: f32, g: f32, b: f32) -> f32
{
	return sqrt(r * r * 0.241 + g * g * 0.691 + b * b * 0.068);
}

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>
{
	let src: vec4<f32> = textureSample(input_tex, input_sampler, in.tex_coords);
	let p: f32 = PerceivedBrightness(src.r, src.g, src.b);
	return vec4<f32>(p, p, p, 1.0);
}