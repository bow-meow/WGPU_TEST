struct VertexOutput{
	@builtin(position) clip_position: vec4<f32>,
	@location(0) tex_coords: vec2<f32>
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

@group(1) @binding(0) var<uniform> data: DataUniform;

struct DataUniform{
	factor: f32
};

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>
{
	var c0: vec4<f32> = textureSample(input_tex, input_sampler, in.tex_coords);
	
	c0.r = (f32(i32(c0.r * data.factor)) / data.factor);
	c0.g = (f32(i32(c0.g * data.factor)) / data.factor);
	c0.b = (f32(i32(c0.b * data.factor)) / data.factor);
	
	return c0;
}