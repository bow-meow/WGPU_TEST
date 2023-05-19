struct VertexOutput{
	@builtin(position) clip_position: vec4<f32>,
	@location(0) tex_coords: vec2<f32>,
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

@group(1) @binding(0) var<uniform> data: DataUniform;

struct DataUniform{
	pixel_size: vec4<f32>
};

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>
{
	let col: vec4<f32> = textureSample(input_tex, input_sampler, in.tex_coords);
	
	let h: vec2<f32> = vec2<f32>(0.0, data.pixel_size.y * 2.0);
	let c1: vec4<f32> = textureSample(input_tex, input_sampler, in.tex_coords - h);
	let c2: vec4<f32> = textureSample(input_tex, input_sampler, in.tex_coords + h);
	return vec4<f32>(col * 2.0 + c1 + c2) / 4.0;
}