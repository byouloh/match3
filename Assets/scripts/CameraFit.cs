using UnityEngine;
using System.Collections;

public class CameraFit : MonoBehaviour
{
	/** Ширина экрана в пикселях. */
	private int _screenWidth;

	/** Высота экрана в пикселях. */
	private int _screenHeight;

	/** Соотношение сторон. */
	private float _actualAspect;

	/** Максимальная ширина видимой части в мировых единицах.  */
	public float actualWidth;

	/** Максимальная высота видимой части в мировых единицах.  */
	public float actualHeight;

	/** Запоминает начальный размер экрана. */
    void Start()
	{
		_screenWidth  = 0;
		_screenHeight = 0;
		_actualAspect = actualWidth / actualHeight; //2
	}

	/** При изменении размера экрана, обновляет масштаб сцены. */
	void Update()
	{
		if (_screenWidth != Screen.width || _screenHeight != Screen.height) {
			_screenWidth  = Screen.width;
			_screenHeight = Screen.height;

			float height = Camera.main.orthographicSize * 2;
			float width = Camera.main.pixelWidth / Camera.main.pixelHeight * Camera.main.orthographicSize * 2;

			float aspect = width / height;

			if (aspect > _actualAspect) {
				float scale = actualWidth / width;
				Camera.main.orthographicSize = Camera.main.orthographicSize * scale;
			} else {
				float scale = actualHeight / height;
				Camera.main.orthographicSize = Camera.main.orthographicSize * scale;
			}
		}
	}

	/** Возвращает ширину видимой части в мировых единицах. */
	public float getWidth()
	{
		return Camera.main.pixelWidth / Camera.main.pixelHeight * Camera.main.orthographicSize * 2;
	}

	/** Возвращает Высоту видимой части в мировых единицах. */
	public float getHeight()
	{
		return Camera.main.orthographicSize * 2;
	}
}
