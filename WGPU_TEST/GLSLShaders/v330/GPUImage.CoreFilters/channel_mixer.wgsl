struct VertexOutput {
    @builtin(position) clip_position: vec4<f32>,
    @location(0) tex_coords: vec2<f32>,
};

// Fragment shader
struct DataUniform{
	r_gain: vec3<f32>,
	g_gain: vec3<f32>,
	b_gain: vec3<f32>
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

@group(1) @binding(0) var<uniform> data: DataUniform;

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>{
	let src: vec4<f32> = textureSample(input_tex, input_sampler, in.tex_coords);

	let r_out: f32 = dot(data.r_gain.rgb, src.rgb);
	let g_out: f32 = dot(data.g_gain.rgb, src.rgb);
	let b_out: f32 = dot(data.b_gain.rgb, src.rgb);
	
	return vec4<f32>(r_out, g_out, b_out, src.a);
}