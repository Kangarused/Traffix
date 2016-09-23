module Traffix.Services {
    export interface IColorService {
        getCongestionColor(state: number): number[];
        calculateIncrement(startColorArray, endColorArray, steps): number[];
        transition(currentColor, targetColor, increment): {hex: string, color: number[]};
        RGB2Hex(colorArray): string;
        RGBtoHSV(color): number[];
        HSVtoRGB(color): number[];
    }

    export class ColorService implements IColorService {
        static $inject = [];
        constructor() { }

        colors = {
            low: [102, 204, 0],
            medium: [204, 153, 0],
            high: [204, 0, 0]
        }

        getCongestionColor(state: number): number[] {
            if (state === 0) {
                return this.colors.low;
            }else if (state === 1) {
                return this.colors.medium;
            }else if (state === 2) {
                return this.colors.high;
            }
        }

        transition(currentColor, targetColor, increment) {
            // checking R
            if (currentColor[0] > targetColor[0]) {
                currentColor[0] -= increment[0];
                if (currentColor[0] <= targetColor[0]) {
                    increment[0] = 0;
                }
            } else {
                currentColor[0] += increment[0];
                if (currentColor[0] >= targetColor[0]) {
                    increment[0] = 0;
                }
            }

            // checking G
            if (currentColor[1] > targetColor[1]) {
                currentColor[1] -= increment[1];
                if (currentColor[1] <= targetColor[1]) {
                    increment[1] = 0;
                }
            } else {
                currentColor[1] += increment[1];
                if (currentColor[1] >= targetColor[1]) {
                    increment[1] = 0;
                }
            }

            // checking B
            if (currentColor[2] > targetColor[2]) {
                currentColor[2] -= increment[2];
                if (currentColor[2] <= targetColor[2]) {
                    increment[2] = 0;
                }
            } else {
                currentColor[2] += increment[2];
                if (currentColor[2] >= targetColor[2]) {
                    increment[2] = 0;
                }
            }
            var rgb = this.HSVtoRGB(currentColor);
            var hex = this.RGB2Hex(rgb);
            return {hex: hex, color: currentColor};
        }

        calculateDistance(colorArray1, colorArray2) {
            var distance = [];
            for (var i = 0; i < colorArray1.length; i++) {
                distance.push(Math.abs(colorArray1[i] - colorArray2[i]));
            }
            return distance;
        }

        calculateIncrement(startColorArray, endColorArray, steps) {
            var distanceArray = this.calculateDistance(startColorArray, endColorArray);
            var increment = [];
            for (var i = 0; i < distanceArray.length; i++) {
                var incr = Math.abs(distanceArray[i] / steps);
                if (incr == 0) {
                    incr = 0;
                }
                increment.push(incr);
            }
            return increment;
        }

        RGB2Hex(colorArray) {
            var color = [];
            for (var i = 0; i < colorArray.length; i++) {
                var hex = colorArray[i].toString(16);
                if (hex.length < 2) { hex = "0" + hex; }
                color.push(hex);
            }
            return "#" + color.join("");
        }

        RGBtoHSV(color) {
            var r = color[0];
            var g = color[1];
            var b = color[2];

            var max = Math.max(r, g, b), min = Math.min(r, g, b),
                d = max - min,
                h,
                s = (max === 0 ? 0 : d / max),
                v = max / 255;

            switch (max) {
                case min: h = 0; break;
                case r: h = (g - b) + d * (g < b ? 6 : 0); h /= 6 * d; break;
                case g: h = (b - r) + d * 2; h /= 6 * d; break;
                case b: h = (r - g) + d * 4; h /= 6 * d; break;
            }

            return [h,s,v];
        }

        HSVtoRGB(color) {
            var h = color[0];
            var s = color[1];
            var v = color[2];

            var r, g, b, i, f, p, q, t;

            i = Math.floor(h * 6);
            f = h * 6 - i;
            p = v * (1 - s);
            q = v * (1 - f * s);
            t = v * (1 - (1 - f) * s);
            switch (i % 6) {
                case 0: r = v, g = t, b = p; break;
                case 1: r = q, g = v, b = p; break;
                case 2: r = p, g = v, b = t; break;
                case 3: r = p, g = q, b = v; break;
                case 4: r = t, g = p, b = v; break;
                case 5: r = v, g = p, b = q; break;
            }
            return [Math.round(r * 255), Math.round(g * 255), Math.round(b * 255)];
        }
    }
}