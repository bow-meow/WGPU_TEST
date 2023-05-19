struct VertexOutput{
	@builtin(position) clip_position: vec4<f32>,
	@location(0) tex_coords: vec2<f32>
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;	
@group(0) @binding(1) var input_sampler: sampler;

@group(1) @binding(0) var<uniform> data: DataUniform;

@group(2) @binding(0) var blurred_mask_tex: texture_2d<f32>;
@group(2) @binding(1) var background_tex: texture_2d<f32>;

struct DataUniform{
	blend: f32
};

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>
{
	let src: vec4<f32> = textureSample(input_tex, input_sampler, in.tex_coords);
	
	let mask_blurred: f32 = textureSample(blurred_mask_tex, input_sampler, in.tex_coords).a;
	let mask: f32 = src.a - mask_blurred;

	let bg: vec4<f32> = textureSample(background_tex, input_sampler, in.tex_coords);

	return vec4<f32>(mix(src.rgb, mix(src.rgb, bg.rgb, mask), data.blend), src.a);
}