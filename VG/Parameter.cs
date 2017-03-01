namespace PiGameSharp.VG
{
	/// <summary>
	/// The OpenVG parameters that can be used
	/// </summary>
	public enum Parameter : uint
	{
		// Context Parameters
		[ParameterInfo(typeof(MatrixMode))]
		MATRIX_MODE                              = 0x1100,
		FILL_RULE                                = 0x1101,
		[ParameterInfo(typeof(ImageRenderQuality))]
		IMAGE_QUALITY                            = 0x1102,
		RENDERING_QUALITY                        = 0x1103,
		[ParameterInfo(typeof(BlendMode))]
		BLEND_MODE                               = 0x1104,
		IMAGE_MODE                               = 0x1105,
		[ParameterInfo(ParameterAccessor.IntArray)]
		SCISSOR_RECTS                            = 0x1106,
		COLOR_TRANSFORM                          = 0x1170,
		COLOR_TRANSFORM_VALUES                   = 0x1171,
		STROKE_LINE_WIDTH                        = 0x1110,
		STROKE_CAP_STYLE                         = 0x1111,
		STROKE_JOIN_STYLE                        = 0x1112,
		STROKE_MITER_LIMIT                       = 0x1113,
		STROKE_DASH_PATTERN                      = 0x1114,
		STROKE_DASH_PHASE                        = 0x1115,
		STROKE_DASH_PHASE_RESET                  = 0x1116,
		[ParameterInfo(ParameterAccessor.FloatArray)]
		TILE_FILL_COLOR                          = 0x1120,
		[ParameterInfo(ParameterAccessor.FloatArray)]
		CLEAR_COLOR                              = 0x1121,
		[ParameterInfo(ParameterAccessor.FloatArray)]
		GLYPH_ORIGIN                             = 0x1122,
		[ParameterInfo(typeof(bool))]
		MASKING                                  = 0x1130,
		[ParameterInfo(typeof(bool))]
		SCISSORING                               = 0x1131,
		PIXEL_LAYOUT                             = 0x1140,
		SCREEN_LAYOUT                            = 0x1141,
		FILTER_FORMAT_LINEAR                     = 0x1150,
		FILTER_FORMAT_PREMULTIPLIED              = 0x1151,
		FILTER_CHANNEL_MASK                      = 0x1152,
		MAX_SCISSOR_RECTS                        = 0x1160,
		MAX_DASH_COUNT                           = 0x1161,
		MAX_KERNEL_SIZE                          = 0x1162,
		MAX_SEPARABLE_KERNEL_SIZE                = 0x1163,
		MAX_COLOR_RAMP_STOPS                     = 0x1164,
		MAX_IMAGE_WIDTH                          = 0x1165,
		MAX_IMAGE_HEIGHT                         = 0x1166,
		MAX_IMAGE_PIXELS                         = 0x1167,
		MAX_IMAGE_BYTES                          = 0x1168,
		[ParameterInfo(typeof(float))]
		MAX_FLOAT                                = 0x1169,
		MAX_GAUSSIAN_STD_DEVIATION               = 0x116A,

		// Path Parameters
		PATH_FORMAT                              = 0x1600,
		PATH_DATATYPE                            = 0x1601,
		PATH_SCALE                               = 0x1602,
		PATH_BIAS                                = 0x1603,
		PATH_NUM_SEGMENTS                        = 0x1604,
		PATH_NUM_COORDS                          = 0x1605,

		// Paint Parameters
		PAINT_TYPE                               = 0x1A00,
		PAINT_COLOR                              = 0x1A01,
		PAINT_COLOR_RAMP_SPREAD_MODE             = 0x1A02,
		PAINT_COLOR_RAMP_PREMULTIPLIED           = 0x1A07,
		PAINT_COLOR_RAMP_STOPS                   = 0x1A03,
		PAINT_LINEAR_GRADIENT                    = 0x1A04,
		PAINT_RADIAL_GRADIENT                    = 0x1A05,
		PAINT_PATTERN_TILING_MODE                = 0x1A06,

		// Image Parameters
		[ParameterInfo(typeof(ImageFormat))]
		IMAGE_FORMAT                             = 0x1E00,
		IMAGE_WIDTH                              = 0x1E01,
		IMAGE_HEIGHT                             = 0x1E02,

		// Font Parameters
		FONT_NUM_GLYPHS                          = 0x2F00

		//PIXEL_LAYOUT_UNKNOWN                     = 0x1300,
		//PIXEL_LAYOUT_RGB_VERTICAL                = 0x1301,
		//PIXEL_LAYOUT_BGR_VERTICAL                = 0x1302,
		//PIXEL_LAYOUT_RGB_HORIZONTAL              = 0x1303,
		//PIXEL_LAYOUT_BGR_HORIZONTAL              = 0x1304,

		//CLEAR_MASK                               = 0x1500,
		//FILL_MASK                                = 0x1501,
		//SET_MASK                                 = 0x1502,
		//UNION_MASK                               = 0x1503,
		//INTERSECT_MASK                           = 0x1504,
		//SUBTRACT_MASK                            = 0x1505,

		//CAP_BUTT                                 = 0x1700,
		//CAP_ROUND                                = 0x1701,
		//CAP_SQUARE                               = 0x1702,

		//JOIN_MITER                               = 0x1800,
		//JOIN_ROUND                               = 0x1801,
		//JOIN_BEVEL                               = 0x1802,

		//EVEN_ODD                                 = 0x1900,
		//NON_ZERO                                 = 0x1901,

		//PAINT_TYPE_COLOR                         = 0x1B00,
		//PAINT_TYPE_LINEAR_GRADIENT               = 0x1B01,
		//PAINT_TYPE_RADIAL_GRADIENT               = 0x1B02,
		//PAINT_TYPE_PATTERN                       = 0x1B03,

		//COLOR_RAMP_SPREAD_PAD                    = 0x1C00,
		//COLOR_RAMP_SPREAD_REPEAT                 = 0x1C01,
		//COLOR_RAMP_SPREAD_REFLECT                = 0x1C02,

		//TILE_FILL                                = 0x1D00,
		//TILE_PAD                                 = 0x1D01,
		//TILE_REPEAT                              = 0x1D02,
		//TILE_REFLECT                             = 0x1D03,
		//DRAW_IMAGE_NORMAL                        = 0x1F00,
		//DRAW_IMAGE_MULTIPLY                      = 0x1F01,
		//DRAW_IMAGE_STENCIL                       = 0x1F02,

		//IMAGE_FORMAT_QUERY                       = 0x2100, 
		//PATH_DATATYPE_QUERY                      = 0x2101,

		//HARDWARE_ACCELERATED                     = 0x2200,
		//HARDWARE_UNACCELERATED                   = 0x2201,
	}
}
