struct VertexOutput {
    @builtin(position) clip_position: vec4<f32>,
    @location(0) tex_coords: vec2<f32>,
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>
{
	let src: vec4<f32> = textureSample(input_tex, input_sampler, in.tex_coords);
	var color = vec4<f32>(1.0, 1.0, 1.0, 1.0);

	if (src.a <= 0.0){
		color *= 0.0;
	}
	else if (src.a < 0.25){
		color *= 0.166666;
	}
	else if (src.a < 0.5){
		color *= 0.333333;
	}
	else if (src.a < 0.75){
		color *= 0.5;
	}	
	else if (src.a < 1.0){
		color *= 0.6555;
	}
	else{
		color *= 1.0;
	}
		
	color.a = 1.0;
	return color;
}