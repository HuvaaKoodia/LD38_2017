using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Contains a bunch of helper methods for all sorts of tasks.
/// </summary>
public static class Helpers
{
	/// <summary>
	/// Random int from 0 to max (ex).
	/// </summary>
	public static int Rand(int max)
	{
		return Random.Range(0, max);
	}

	/// <summary>
	/// Random int from min to max (ex).
	/// </summary>
	public static int Rand(int min, int max)
	{
		return Random.Range(min, max);
	}

	/// <summary>
	/// Random float from 0 to max (inc).
	/// </summary>
	public static float Rand(float max)
	{
		return Random.Range(0f, max);
	}

	/// <summary>
	/// Random float from min to max (inc).
	/// </summary>
	public static float Rand(float min, float max)
	{
		return Random.Range(min, max);
	}

	/// <summary>
	/// Random float from 0f to 1f(ex).
	/// </summary>
	public static float RandFloat()
	{
		return Random.Range(0f, 1f);
	}
	/// <summary>
	/// Random int from 0 to 100(ex)
	/// </summary>
	public static int RandPercent()
	{
		return Random.Range(0, 100);
	}

	/// <summary>
	/// A percent check
	/// </summary>
	public static bool RandPercent(int chance)
	{
		return RandPercent() < chance;
	}

	public static bool RandBool()
	{
		return RandPercent() < 50;
	}
	/// <summary>
	/// Random vector3. All values from -1f to 1f.
	/// </summary>
	public static Vector3 RandVector3()
	{
		return new Vector3(Random.Range(-1f, 1), Random.Range(-1f, 1), Random.Range(-1f, 1));
	}

	public static Color RandColor()
	{
		return new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
	}

	public static Color RandColor(float alpha)
	{
		return new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), alpha);
	}

	public static T RandEnum<T>()
	{
		return Rand(EnumValues<T>());
	}

	public static T RandParam<T>(params T[] ts)
	{
		if (ts.Count() == 0)
		{
			Debug.LogError("");
		}

		return ts.ElementAt(Rand(ts.Count()));
	}

	public static T Rand<T>(IEnumerable<T> enumerable)
	{
		if (enumerable.Count() == 0)
		{
			Debug.LogError("");
		}

		return enumerable.ElementAt(Rand(enumerable.Count()));
	}

	public static T RandRemove<T>(List<T> list)
	{
		T v = Rand(list);
		list.Remove(v);
		return v;
	}
    
    //strings
	public static string[] Split(string str, string separator, bool removeEmpty = true, bool trimEntries = false)
	{
        var split = str.Split(new string[] { separator }, removeEmpty ? System.StringSplitOptions.RemoveEmptyEntries : System.StringSplitOptions.None);

        if (trimEntries) {
            for (int i = 0; i < split.Length; i++)
            {
                split[i] = split[i].Trim();
            }
        }

        return split;
    }

    //distance
	public static float DistanceSquared(Vector2 pos1, Vector2 pos2)
	{
		float dx = pos1.x - pos2.x;
		float dy = pos1.y - pos2.y;
		return (int)((dx * dx) + (dy * dy));
	}

    /// <summary>
    /// Changes the enable state of the component if it's not in the same state already.
    /// </summary>
    public static void EnableComponent(Behaviour mb, bool enable)
    {
        if (mb.enabled != enable) mb.enabled = enable;
    }
    /// <summary>
    /// Changes the active state of the object if it's not in the same state already.
    /// </summary>
    public static void SetActive(GameObject obj, bool active)
    {
        if (obj.activeSelf != active) obj.SetActive(active);
    }
    /// <summary>
    /// Gets angle around local y axis of the parent from a world space direction
    /// </summary>
    public static float GetAngleTowardsDirection(Transform parent, Vector3 worldDirection)
    {
        Vector3 local = parent.InverseTransformDirection(worldDirection);
        return Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg;
    }
    /// <summary>
    /// Returns a signed angle between two direction vectors around an arbitrary axis
    /// </summary>
    public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 axis)
    {
        return Mathf.Atan2(Vector3.Dot(axis, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// Returns all values of an enum in an iterable collection. Usable in a foreach loop.
    /// </summary>
    public static IEnumerable<T> EnumValues<T>()
    {
        return System.Enum.GetValues(typeof(T)).Cast<T>();
    }

    //Unity specifics
    public static void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Vector3 extensions
    /// <summary>
    /// Returns the direction vector from from to to.
    /// </summary>
    public static Vector3 To(this Vector3 from, Vector3 to)
    {
        return (to - from);
    }

    /// <summary>
    /// Returns the normalized direction vector from from to to.
    /// </summary>
    public static Vector3 ToNorm(this Vector3 from, Vector3 to)
    {
        return (to - from).normalized;
    }

    /// <summary>
    /// Returns the distance from from to to.
    /// </summary>
    public static float ToDistance(this Vector3 from, Vector3 to)
    {
        return Vector3.Distance(from, to);
    }

    //Transform extensions
    /// <summary>
    /// Returns the direction vector from from to to.
    /// </summary>
    public static Vector3 To(this Transform from, Transform to)
    {
        return (to.position - from.position);
    }

    /// <summary>
    /// Returns the normalized direction vector from from to to.
    /// </summary>
    public static Vector3 ToNorm(this Transform from, Transform to)
    {
        return (to.position - from.position).normalized;
    }

    /// <summary>
    /// Returns the distance from from to to.
    /// </summary>
    public static float ToDistance(this Transform from, Transform to)
    {
        return Vector3.Distance(from.position, to.position);
    }

    //Ray extensions

    /// <summary>
    /// Draws the ray with a color and distance.
    /// Debug use only.
    /// </summary>
    public static void Draw(this Ray ray, float distance, Color color)
    {
        Debug.DrawRay(ray.origin, ray.direction * distance, color);
    }

    /// <summary>
    /// Draws the ray with a color.
    /// Debug use only.
    /// </summary>
    public static void Draw(this Ray ray, Color color)
    {
        Debug.DrawRay(ray.origin, ray.direction, color);
    }

    public static bool ScreenPointToWorldPoint(out Vector3 worldPos, int layerMask)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit info;
        if (Physics.Raycast(ray, out info, 50, layerMask))
        {
            worldPos = info.point;
            return true;
        }
        worldPos = Vector3.zero;
        return false;
    }

    public static bool ScreenPointToObject<T>(out T obj, int layerMask)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit info;
        if (Physics.Raycast(ray, out info, 50, layerMask))
        {
            obj = info.transform.GetComponentInParent<T>();
            return true;
        }
        obj = default(T);
        return false;
    }
}
