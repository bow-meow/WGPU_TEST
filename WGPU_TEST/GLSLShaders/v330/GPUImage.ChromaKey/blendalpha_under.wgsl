struct VertexOutput{
	@builtin(position) clip_position: vec4<f32>,
	@location(0) tex_coords: vec2<f32>
};

@group(0) @binding(0) var fg_tex: texture_2d<f32>;
@group(0) @binding(1) var bg_tex: texture_2d<f32>;
@group(0) @binding(2) var input_sampler: sampler;

fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>
{
	let fg: vec4<f32> = textureSample(fg_tex, input_sampler, in.clip_position.xy);// not sure
	let bg: vec4<f32> = textureSample(bg_tex, input_sampler in.clip_position.zw); // not sure about clip pos
	var color: vec4<f32> = mix(bg.rgb, fg.rgb, fg.a);
	color.a = 1.0;
	return color;
}