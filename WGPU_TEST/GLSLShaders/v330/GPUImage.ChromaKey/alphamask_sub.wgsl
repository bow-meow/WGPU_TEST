struct VertexOutput{
	@builtin(position) clip_position: vec4<f32>,
	@location(0) tex_coords: vec2<f32>
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;
@group(0) @binding(2) var mask_tex: texture_2d<f32>;

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>
{
	let fg: vec4<f32> = textureSample(input_tex, input_sampler, in.tex_coords);
	let mask: f32 = textureSample(mask_tex, input_sampler, in.tex_coords).a;
	return vec4<f32>(fg.xyz, fg.a - mask);
}