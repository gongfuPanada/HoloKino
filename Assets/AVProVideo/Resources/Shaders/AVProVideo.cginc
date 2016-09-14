//-----------------------------------------------------------------------------
// Copyright 2015-2016 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

bool IsStereoEyeLeft(float3 worldNosePosition)
{
#ifdef UNITY_SINGLE_PASS_STEREO
	// Unity 5.4 has this new variable
	return (unity_StereoEyeIndex == 0);
#else
	// worldNosePosition is the camera positon passed in from Unity via script
	// We need to determine whether _WorldSpaceCameraPos (Unity shader variable) is to the left or to the right of _cameraPosition
	float3 right = UNITY_MATRIX_V[0].xyz;
	float dRight = distance(worldNosePosition + right, _WorldSpaceCameraPos);
	float dLeft = distance(worldNosePosition - right, _WorldSpaceCameraPos);
	return (dRight > dLeft);
#endif
}

float2 ScaleZoomToFit(float targetWidth, float targetHeight, float sourceWidth, float sourceHeight)
{
#if ALPHAPACK_TOP_BOTTOM
	sourceHeight /= 2.0;
#elif ALPHAPACK_LEFT_RIGHT
	sourceWidth /= 2.0;
#endif
	float targetAspect = targetHeight / targetWidth;
	float sourceAspect = sourceHeight / sourceWidth;
	float2 scale = float2(1.0, sourceAspect / targetAspect);
	if (targetAspect < sourceAspect)
	{
		scale = float2(targetAspect / sourceAspect, 1.0);
	}
	return scale;
}

float4 OffsetAlphaPackingUV(float2 texelSize, float2 uv, bool flipVertical)
{
	float4 result = uv.xyxy;

	// We don't want bilinear interpolation to cause bleeding
	// when reading the pixels at the edge of the packed areas.
	// So we shift the UV's by a fraction of a pixel so the edges don't get sampled.

#if ALPHAPACK_TOP_BOTTOM
	float offset = texelSize.y * 1.5;
	result.y = lerp(0.0 + offset, 0.5 - offset, uv.y);
	result.w = result.y + 0.5;

	if (flipVertical)
	{
		// Flip vertically (and offset to put back in 0..1 range)
		result.yw = 1.0 - result.yw;
		result.yw = result.wy;
	}
	else
	{
#if !UNITY_UV_STARTS_AT_TOP
		// For opengl we flip
		result.yw = result.wy;
#endif
	}

#elif ALPHAPACK_LEFT_RIGHT
	float offset = texelSize.x * 1.5;
	result.x = lerp(0.0 + offset, 0.5 - offset, uv.x);
	result.z = result.x + 0.5;

	if (flipVertical)
	{
		// Flip vertically (and offset to put back in 0..1 range)
		result.yw = 1.0 - result.yw;
	}

#endif

	return result;
}