struct VertexOutput{
	@builtin(position) clip_position: vec4<f32>,
	@location(0) tex_coords: vec2<f32>
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

@group(1) @binding(0) var<uniform> data: DataUniform;

struct DataUniform{
	in_black: f32,
	in_white: f32,
	in_gamma: f32,
	out_white: f32,
	out_black: f32
};

fn Levels(color: vec4<f32>) -> vec4<f32>
{
	let one = vec4<f32>(1.0, 1.0f, 1.0f, 1.0f);
	let zero = vec4<f32>(0.0, 0.0f, 0.0f, 0.0f);
	let i: vec4<f32> = min(one, (max(color - data.in_black, zero) / (data.in_white - data.in_black)));
	var p = vec4<f32>(pow(i.x, data.in_gamma), pow(i.y, data.in_gamma), pow(i.z, data.in_gamma), pow(i.w, data.in_gamma));
	p = (p * (data.out_white - data.out_black)) + data.out_black;
	return p;
}

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>
{
	let src: vec4<f32> = textureSample(input_tex, input_sampler, in.tex_coords);

	let s: vec4<f32> = Levels(src.wwww);

	return vec4<f32>(src.xyz, s.x);
}