using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {

    public static bool inRange (float value, float from, float to) {
        return value >= from && value <= to;
    }

    public static bool outRange (float value, float from, float to) {
        return !inRange(value, from, to);
    }

    public static float tendingToLimit (
        float value, float rate, float limit = 0
    ) {

        if (Mathf.Abs(value - limit) < rate) {
            return limit;
        }
        if (value > limit) {
            return value - rate;
        } else {
            return value + rate;
        }
    }

    public static bool equalFloat (float a, float b) {
        return Mathf.Abs(a - b) < Mathf.Epsilon;
    }
}
