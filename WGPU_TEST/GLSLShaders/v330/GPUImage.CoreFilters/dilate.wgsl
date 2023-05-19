struct VertexOutput{
	@builtin(position) clip_position: vec4<f32>,
	@location(0) tex_coords: vec2<f32>,
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

@group(1) @binding(0) var<uniform> data: DataUniform;

struct DataUniform{
	pixel_size: vec2<f32>,
	strength: f32
};

struct QuadSampleResult{
	c0: vec4<f32>,
	c1: vec4<f32>,
	c2: vec4<f32>,
	c3: vec4<f32>,
	c4: vec4<f32>
};

fn QuadSample(uvCenter: vec2<f32>, uvOffset: vec2<f32>) -> QuadSampleResult {
	var ret: QuadSampleResult;
	ret.c0 = textureSample(input_tex, input_sampler, uvCenter);
    ret.c1 = textureSample(input_tex, input_sampler, uvCenter + vec2<f32>(-uvOffset.x, uvOffset.y));
	ret.c2 = textureSample(input_tex, input_sampler, uvCenter + vec2<f32>(uvOffset.x, uvOffset.y));
	ret.c3 = textureSample(input_tex, input_sampler, uvCenter + vec2<f32>(uvOffset.x, -uvOffset.y));
	ret.c4 = textureSample(input_tex, input_sampler, uvCenter - vec2<f32>(uvOffset.x, uvOffset.y));

	return ret;
}

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>{
	let quad_sample: QuadSampleResult = QuadSample(in.tex_coords, data.pixel_size.xy * 1.0);
	
	return mix(quad_sample.c0, max(max(quad_sample.c1, quad_sample.c2), max(quad_sample.c3, quad_sample.c4)), data.strength);
}