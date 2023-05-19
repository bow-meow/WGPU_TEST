struct VertexOutput {
    @builtin(position) clip_position: vec4<f32>,
    @location(0) tex_coords: vec2<f32>,
};

// Fragment shader

struct DataUniform{
	focal_length: f32,
	principal_x: f32,
	principal_y: f32,
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

@group(1) @binding(0) var<uniform> data: DataUniform;

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>{
	let k1: f32 = 0.4142;    // constant for radial distortion correction
	let k2: f32 = 0.40348;
	var xy: vec2<f32> = (in.tex_coords - vec2<f32>(data.principal_x, data.principal_y))/vec2(data.focal_length,data.focal_length);

	let r:		f32 = sqrt( dot(xy, xy) );
	let r2:		f32 = r * r;
	let r4:		f32 = r2 * r2;
	let coeff:	f32 = (k1 * r2 + k2 * r4);

	// add the calculated offsets to the current texture coordinates
	xy = ((xy + xy * coeff) * data.focal_length) + vec2(data.principal_x, data.principal_y); // ALEX MOD: changed coeff.xx & focal_length.xx to just multiply by the float as floats arent vectors so they dont have .xx methods

	// look up the texture at the corrected coordinates
	// and output the color
	return textureSample(input_tex, input_sampler, xy);
}