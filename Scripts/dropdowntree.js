! function(e, define) {
    define("kendo.core.min", ["jquery"], e)
}(function() {
    return function(e, t, n) {
        function i() {}

        function o(e, t) {
            if (t) return "'" + e.split("'").join("\\'").split('\\"').join('\\\\\\"').replace(/\n/g, "\\n").replace(/\r/g, "\\r").replace(/\t/g, "\\t") + "'";
            var n = e.charAt(0),
                i = e.substring(1);
            return "=" === n ? "+(" + i + ")+" : ":" === n ? "+$kendoHtmlEncode(" + i + ")+" : ";" + e + ";$kendoOutput+="
        }

        function r(e, t, n) {
            return e += "", t = t || 2, n = t - e.length, n ? W[t].substring(0, n) + e : e
        }

        function s(e) {
            var t = e.css(ve.support.transitions.css + "box-shadow") || e.css("box-shadow"),
                n = t ? t.match(Ae) || [0, 0, 0, 0, 0] : [0, 0, 0, 0, 0],
                i = xe.max(+n[3], +(n[4] || 0));
            return {
                left: -n[1] + i,
                right: +n[1] + i,
                bottom: +n[2] + i
            }
        }

        function a(t, n) {
            var i, o, r, s, a, l, c, d = Se.browser,
                u = ve._outerWidth,
                h = ve._outerHeight;
            return t.parent().hasClass("k-animation-container") ? (l = t.parent(".k-animation-container"), c = l[0].style, l.is(":hidden") && l.css({
                display: "",
                position: ""
            }), i = Te.test(c.width) || Te.test(c.height), i || l.css({
                width: n ? u(t) + 1 : u(t),
                height: h(t),
                boxSizing: "content-box",
                mozBoxSizing: "content-box",
                webkitBoxSizing: "content-box"
            })) : (o = t[0].style.width, r = t[0].style.height, s = Te.test(o), a = Te.test(r), i = s || a, !s && (!n || n && o) && (o = n ? u(t) + 1 : u(t)), !a && (!n || n && r) && (r = h(t)), t.wrap(e("<div/>").addClass("k-animation-container").css({
                width: o,
                height: r
            })), i && t.css({
                width: "100%",
                height: "100%",
                boxSizing: "border-box",
                mozBoxSizing: "border-box",
                webkitBoxSizing: "border-box"
            })), d.msie && xe.floor(d.version) <= 7 && (t.css({
                zoom: 1
            }), t.children(".k-menu").width(t.width())), t.parent()
        }

        function l(e) {
            var t = 1,
                n = arguments.length;
            for (t = 1; t < n; t++) c(e, arguments[t]);
            return e
        }

        function c(e, t) {
            var n, i, o, r, s, a = ve.data.ObservableArray,
                l = ve.data.LazyObservableArray,
                d = ve.data.DataSource,
                u = ve.data.HierarchicalDataSource;
            for (n in t) i = t[n], o = typeof i, r = o === Fe && null !== i ? i.constructor : null, r && r !== Array && r !== a && r !== l && r !== d && r !== u && r !== RegExp ? i instanceof Date ? e[n] = new Date(i.getTime()) : R(i.clone) ? e[n] = i.clone() : (s = e[n], e[n] = typeof s === Fe ? s || {} : {}, c(e[n], i)) : o !== Be && (e[n] = i);
            return e
        }

        function d(e, t, i) {
            for (var o in t)
                if (t.hasOwnProperty(o) && t[o].test(e)) return o;
            return i !== n ? i : e
        }

        function u(e) {
            return e.replace(/([a-z][A-Z])/g, function(e) {
                return e.charAt(0) + "-" + e.charAt(1).toLowerCase()
            })
        }

        function h(e) {
            return e.replace(/\-(\w)/g, function(e, t) {
                return t.toUpperCase()
            })
        }

        function p(t, n) {
            var i, o = {};
            return document.defaultView && document.defaultView.getComputedStyle ? (i = document.defaultView.getComputedStyle(t, ""), n && e.each(n, function(e, t) {
                o[t] = i.getPropertyValue(t)
            })) : (i = t.currentStyle, n && e.each(n, function(e, t) {
                o[t] = i[h(t)]
            })), ve.size(o) || (o = i), o
        }

        function f(e) {
            if (e && e.className && "string" == typeof e.className && e.className.indexOf("k-auto-scrollable") > -1) return !0;
            var t = p(e, ["overflow"]).overflow;
            return "auto" == t || "scroll" == t
        }

        function m(t, i) {
            var o, r = Se.browser.webkit,
                s = Se.browser.mozilla,
                a = t instanceof e ? t[0] : t;
            if (t) return o = Se.isRtl(t), i === n ? o && r ? a.scrollWidth - a.clientWidth - a.scrollLeft : Math.abs(a.scrollLeft) : (a.scrollLeft = o && r ? a.scrollWidth - a.clientWidth - i : o && s ? -i : i, n)
        }

        function g(e) {
            var t, n = 0;
            for (t in e) e.hasOwnProperty(t) && "toJSON" != t && n++;
            return n
        }

        function v(e, n, i) {
            var o, r, s;
            return n || (n = "offset"), o = e[n](), r = {
                top: o.top,
                right: o.right,
                bottom: o.bottom,
                left: o.left
            }, Se.browser.msie && (Se.pointers || Se.msPointers) && !i && (s = Se.isRtl(e) ? 1 : -1, r.top -= t.pageYOffset - document.documentElement.scrollTop, r.left -= t.pageXOffset + s * document.documentElement.scrollLeft), r
        }

        function _(e) {
            var t = {};
            return be("string" == typeof e ? e.split(" ") : e, function(e) {
                t[e] = this
            }), t
        }

        function b(e) {
            return new ve.effects.Element(e)
        }

        function w(e, t, n, i) {
            return typeof e === Me && (R(t) && (i = t, t = 400, n = !1), R(n) && (i = n, n = !1), typeof t === ze && (n = t, t = 400), e = {
                effects: e,
                duration: t,
                reverse: n,
                complete: i
            }), _e({
                effects: {},
                duration: 400,
                reverse: !1,
                init: ke,
                teardown: ke,
                hide: !1
            }, e, {
                completeCallback: e.complete,
                complete: ke
            })
        }

        function y(t, n, i, o, r) {
            for (var s, a = 0, l = t.length; a < l; a++) s = e(t[a]), s.queue(function() {
                q.promise(s, w(n, i, o, r))
            });
            return t
        }

        function k(e, t, n, i) {
            return t && (t = t.split(" "), be(t, function(t, n) {
                e.toggleClass(n, i)
            })), e
        }

        function x(e) {
            return ("" + e).replace(j, "&amp;").replace(G, "&lt;").replace(Y, "&gt;").replace($, "&quot;").replace(K, "&#39;")
        }

        function C(e, t) {
            var i;
            return 0 === t.indexOf("data") && (t = t.substring(4), t = t.charAt(0).toLowerCase() + t.substring(1)), t = t.replace(oe, "-$1"), i = e.getAttribute("data-" + ve.ns + t), null === i ? i = n : "null" === i ? i = null : "true" === i ? i = !0 : "false" === i ? i = !1 : Ee.test(i) && "mask" != t ? i = parseFloat(i) : ne.test(i) && !ie.test(i) && (i = Function("return (" + i + ")")()), i
        }

        function S(t, i, o) {
            var r, s, a = {};
            for (r in i) s = C(t, r), s !== n && (te.test(r) && ("string" == typeof s ? e("#" + s).length ? s = ve.template(e("#" + s).html()) : o && (s = ve.template(o[s])) : s = t.getAttribute(r)), a[r] = s);
            return a
        }

        function T(t, n) {
            return e.contains(t, n) ? -1 : 1
        }

        function D() {
            var t = e(this);
            return e.inArray(t.attr("data-" + ve.ns + "role"), ["slider", "rangeslider"]) > -1 || t.is(":visible")
        }

        function A(e, t) {
            var n = e.nodeName.toLowerCase();
            return (/input|select|textarea|button|object/.test(n) ? !e.disabled : "a" === n ? e.href || t : t) && E(e)
        }

        function E(t) {
            return e.expr.filters.visible(t) && !e(t).parents().addBack().filter(function() {
                return "hidden" === e.css(this, "visibility")
            }).length
        }

        function I(e, t) {
            return new I.fn.init(e, t)
        }
        var M, R, F, P, z, B, L, H, N, O, V, W, U, q, j, G, $, K, Y, Q, X, Z, J, ee, te, ne, ie, oe, re, se, ae, le, ce, de, ue, he, pe, fe, me, ge, ve = t.kendo = t.kendo || {
                cultures: {}
            },
            _e = e.extend,
            be = e.each,
            we = e.isArray,
            ye = e.proxy,
            ke = e.noop,
            xe = Math,
            Ce = t.JSON || {},
            Se = {},
            Te = /%/,
            De = /\{(\d+)(:[^\}]+)?\}/g,
            Ae = /(\d+(?:\.?)\d*)px\s*(\d+(?:\.?)\d*)px\s*(\d+(?:\.?)\d*)px\s*(\d+)?/i,
            Ee = /^(\+|-?)\d+(\.?)\d*$/,
            Ie = "function",
            Me = "string",
            Re = "number",
            Fe = "object",
            Pe = "null",
            ze = "boolean",
            Be = "undefined",
            Le = {},
            He = {},
            Ne = [].slice;
        ve.version = "2018.3.911".replace(/^\s+|\s+$/g, ""), i.extend = function(e) {
                var t, n, i = function() {},
                    o = this,
                    r = e && e.init ? e.init : function() {
                        o.apply(this, arguments)
                    };
                i.prototype = o.prototype, n = r.fn = r.prototype = new i;
                for (t in e) n[t] = null != e[t] && e[t].constructor === Object ? _e(!0, {}, i.prototype[t], e[t]) : e[t];
                return n.constructor = r, r.extend = o.extend, r
            }, i.prototype._initOptions = function(e) {
                this.options = l({}, this.options, e)
            }, R = ve.isFunction = function(e) {
                return "function" == typeof e
            }, F = function() {
                this._defaultPrevented = !0
            }, P = function() {
                return this._defaultPrevented === !0
            }, z = i.extend({
                init: function() {
                    this._events = {}
                },
                bind: function(e, t, i) {
                    var o, r, s, a, l, c = this,
                        d = typeof e === Me ? [e] : e,
                        u = typeof t === Ie;
                    if (t === n) {
                        for (o in e) c.bind(o, e[o]);
                        return c
                    }
                    for (o = 0, r = d.length; o < r; o++) e = d[o], a = u ? t : t[e], a && (i && (s = a, a = function() {
                        c.unbind(e, a), s.apply(c, arguments)
                    }, a.original = s), l = c._events[e] = c._events[e] || [], l.push(a));
                    return c
                },
                one: function(e, t) {
                    return this.bind(e, t, !0)
                },
                first: function(e, t) {
                    var n, i, o, r, s = this,
                        a = typeof e === Me ? [e] : e,
                        l = typeof t === Ie;
                    for (n = 0, i = a.length; n < i; n++) e = a[n], o = l ? t : t[e], o && (r = s._events[e] = s._events[e] || [], r.unshift(o));
                    return s
                },
                trigger: function(e, t) {
                    var n, i, o = this,
                        r = o._events[e];
                    if (r) {
                        for (t = t || {}, t.sender = o, t._defaultPrevented = !1, t.preventDefault = F, t.isDefaultPrevented = P, r = r.slice(), n = 0, i = r.length; n < i; n++) r[n].call(o, t);
                        return t._defaultPrevented === !0
                    }
                    return !1
                },
                unbind: function(e, t) {
                    var i, o = this,
                        r = o._events[e];
                    if (e === n) o._events = {};
                    else if (r)
                        if (t)
                            for (i = r.length - 1; i >= 0; i--) r[i] !== t && r[i].original !== t || r.splice(i, 1);
                        else o._events[e] = [];
                    return o
                }
            }), B = /^\w+/, L = /\$\{([^}]*)\}/g, H = /\\\}/g, N = /__CURLY__/g, O = /\\#/g, V = /__SHARP__/g, W = ["", "0", "00", "000", "0000"], M = {
                paramName: "data",
                useWithBlock: !0,
                render: function(e, t) {
                    var n, i, o = "";
                    for (n = 0, i = t.length; n < i; n++) o += e(t[n]);
                    return o
                },
                compile: function(e, t) {
                    var n, i, r, s = _e({}, this, t),
                        a = s.paramName,
                        l = a.match(B)[0],
                        c = s.useWithBlock,
                        d = "var $kendoOutput, $kendoHtmlEncode = kendo.htmlEncode;";
                    if (R(e)) return e;
                    for (d += c ? "with(" + a + "){" : "", d += "$kendoOutput=", i = e.replace(H, "__CURLY__").replace(L, "#=$kendoHtmlEncode($1)#").replace(N, "}").replace(O, "__SHARP__").split("#"), r = 0; r < i.length; r++) d += o(i[r], r % 2 === 0);
                    d += c ? ";}" : ";", d += "return $kendoOutput;", d = d.replace(V, "#");
                    try {
                        return n = Function(l, d), n._slotCount = Math.floor(i.length / 2), n
                    } catch (u) {
                        throw Error(ve.format("Invalid template:'{0}' Generated code:'{1}'", e, d))
                    }
                }
            },
            function() {
                function e(e) {
                    return s.lastIndex = 0, s.test(e) ? '"' + e.replace(s, function(e) {
                        var t = a[e];
                        return typeof t === Me ? t : "\\u" + ("0000" + e.charCodeAt(0).toString(16)).slice(-4)
                    }) + '"' : '"' + e + '"'
                }

                function t(r, s) {
                    var a, c, d, u, h, p, f = n,
                        m = s[r];
                    if (m && typeof m === Fe && typeof m.toJSON === Ie && (m = m.toJSON(r)), typeof o === Ie && (m = o.call(s, r, m)), p = typeof m, p === Me) return e(m);
                    if (p === Re) return isFinite(m) ? m + "" : Pe;
                    if (p === ze || p === Pe) return m + "";
                    if (p === Fe) {
                        if (!m) return Pe;
                        if (n += i, h = [], "[object Array]" === l.apply(m)) {
                            for (u = m.length, a = 0; a < u; a++) h[a] = t(a, m) || Pe;
                            return d = 0 === h.length ? "[]" : n ? "[\n" + n + h.join(",\n" + n) + "\n" + f + "]" : "[" + h.join(",") + "]", n = f, d
                        }
                        if (o && typeof o === Fe)
                            for (u = o.length, a = 0; a < u; a++) typeof o[a] === Me && (c = o[a], d = t(c, m), d && h.push(e(c) + (n ? ": " : ":") + d));
                        else
                            for (c in m) Object.hasOwnProperty.call(m, c) && (d = t(c, m), d && h.push(e(c) + (n ? ": " : ":") + d));
                        return d = 0 === h.length ? "{}" : n ? "{\n" + n + h.join(",\n" + n) + "\n" + f + "}" : "{" + h.join(",") + "}", n = f, d
                    }
                }
                var n, i, o, s = /[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,
                    a = {
                        "\b": "\\b",
                        "\t": "\\t",
                        "\n": "\\n",
                        "\f": "\\f",
                        "\r": "\\r",
                        '"': '\\"',
                        "\\": "\\\\"
                    },
                    l = {}.toString;
                typeof Date.prototype.toJSON !== Ie && (Date.prototype.toJSON = function() {
                    var e = this;
                    return isFinite(e.valueOf()) ? r(e.getUTCFullYear(), 4) + "-" + r(e.getUTCMonth() + 1) + "-" + r(e.getUTCDate()) + "T" + r(e.getUTCHours()) + ":" + r(e.getUTCMinutes()) + ":" + r(e.getUTCSeconds()) + "Z" : null
                }, String.prototype.toJSON = Number.prototype.toJSON = Boolean.prototype.toJSON = function() {
                    return this.valueOf()
                }), typeof Ce.stringify !== Ie && (Ce.stringify = function(e, r, s) {
                    var a;
                    if (n = "", i = "", typeof s === Re)
                        for (a = 0; a < s; a += 1) i += " ";
                    else typeof s === Me && (i = s);
                    if (o = r, r && typeof r !== Ie && (typeof r !== Fe || typeof r.length !== Re)) throw Error("JSON.stringify");
                    return t("", {
                        "": e
                    })
                })
            }(),
            function() {
                function t(e) {
                    if (e) {
                        if (e.numberFormat) return e;
                        if (typeof e === Me) {
                            var t = ve.cultures;
                            return t[e] || t[e.split("-")[0]] || null
                        }
                        return null
                    }
                    return null
                }

                function i(e) {
                    return e && (e = t(e)), e || ve.cultures.current
                }

                function o(e, t, o) {
                    o = i(o);
                    var s = o.calendars.standard,
                        a = s.days,
                        l = s.months;
                    return t = s.patterns[t] || t, t.replace(d, function(t) {
                        var i, o, c;
                        return "d" === t ? o = e.getDate() : "dd" === t ? o = r(e.getDate()) : "ddd" === t ? o = a.namesAbbr[e.getDay()] : "dddd" === t ? o = a.names[e.getDay()] : "M" === t ? o = e.getMonth() + 1 : "MM" === t ? o = r(e.getMonth() + 1) : "MMM" === t ? o = l.namesAbbr[e.getMonth()] : "MMMM" === t ? o = l.names[e.getMonth()] : "yy" === t ? o = r(e.getFullYear() % 100) : "yyyy" === t ? o = r(e.getFullYear(), 4) : "h" === t ? o = e.getHours() % 12 || 12 : "hh" === t ? o = r(e.getHours() % 12 || 12) : "H" === t ? o = e.getHours() : "HH" === t ? o = r(e.getHours()) : "m" === t ? o = e.getMinutes() : "mm" === t ? o = r(e.getMinutes()) : "s" === t ? o = e.getSeconds() : "ss" === t ? o = r(e.getSeconds()) : "f" === t ? o = xe.floor(e.getMilliseconds() / 100) : "ff" === t ? (o = e.getMilliseconds(), o > 99 && (o = xe.floor(o / 10)), o = r(o)) : "fff" === t ? o = r(e.getMilliseconds(), 3) : "tt" === t ? o = e.getHours() < 12 ? s.AM[0] : s.PM[0] : "zzz" === t ? (i = e.getTimezoneOffset(), c = i < 0, o = ("" + xe.abs(i / 60)).split(".")[0], i = xe.abs(i) - 60 * o, o = (c ? "+" : "-") + r(o), o += ":" + r(i)) : "zz" !== t && "z" !== t || (o = e.getTimezoneOffset() / 60, c = o < 0, o = ("" + xe.abs(o)).split(".")[0], o = (c ? "+" : "-") + ("zz" === t ? r(o) : o)), o !== n ? o : t.slice(1, t.length - 1)
                    })
                }

                function s(e, t, o) {
                    var r, s, c, d, w, y, k, x, C, S, T, D, A, E, I, M, R, F, P, z, B, L, H, N, O, V, W, U, q, j, G, $, K, Y;
                    if (o = i(o), r = o.numberFormat, s = r[m], c = r.decimals, d = r.pattern[0], w = [], T = e < 0, M = f, R = f, G = -1, e === n) return f;
                    if (!isFinite(e)) return e;
                    if (!t) return o.name.length ? e.toLocaleString() : "" + e;
                    if (S = u.exec(t)) {
                        if (t = S[1].toLowerCase(), k = "c" === t, x = "p" === t, (k || x) && (r = k ? r.currency : r.percent, s = r[m], c = r.decimals, y = r.symbol, d = r.pattern[T ? 0 : 1]), C = S[2], C && (c = +C), "e" === t) return K = C ? e.toExponential(c) : e.toExponential(), K.replace(m, r[m]);
                        if (x && (e *= 100), e = l(e, c), T = e < 0, e = e.split(m), D = e[0], A = e[1], T && (D = D.substring(1)), R = a(D, 0, D.length, r), A && (R += s + A), "n" === t && !T) return R;
                        for (e = f, F = 0, P = d.length; F < P; F++) z = d.charAt(F), e += "n" === z ? R : "$" === z || "%" === z ? y : z;
                        return e
                    }
                    if ((t.indexOf("'") > -1 || t.indexOf('"') > -1 || t.indexOf("\\") > -1) && (t = t.replace(h, function(e) {
                            var t = e.charAt(0).replace("\\", ""),
                                n = e.slice(1).replace(t, "");
                            return w.push(n), b
                        })), t = t.split(";"), T && t[1]) t = t[1], L = !0;
                    else if (0 === e && t[2]) {
                        if (t = t[2], t.indexOf(v) == -1 && t.indexOf(_) == -1) return t
                    } else t = t[0];
                    if (U = t.indexOf("%"), q = t.indexOf("$"), x = U != -1, k = q != -1, x && (e *= 100), k && "\\" === t[q - 1] && (t = t.split("\\").join(""), k = !1), (k || x) && (r = k ? r.currency : r.percent, s = r[m], c = r.decimals, y = r.symbol), B = t.indexOf(g) > -1, B && (t = t.replace(p, f)), H = t.indexOf(m), P = t.length, H != -1)
                        if (A = ("" + e).split("e"), A = A[1] ? l(e, Math.abs(A[1])) : A[0], A = A.split(m)[1] || f, O = t.lastIndexOf(_) - H, N = t.lastIndexOf(v) - H, V = O > -1, W = N > -1, F = A.length, V || W || (t = t.substring(0, H) + t.substring(H + 1), P = t.length, H = -1, F = 0), V && O > N) F = O;
                        else if (N > O)
                        if (W && F > N) {
                            for (Y = l(e, N, T); Y.charAt(Y.length - 1) === _ && N > 0 && N > O;) N--, Y = l(e, N, T);
                            F = N
                        } else V && F < O && (F = O);
                    if (e = l(e, F, T), N = t.indexOf(v), j = O = t.indexOf(_), G = N == -1 && O != -1 ? O : N != -1 && O == -1 ? N : N > O ? O : N, N = t.lastIndexOf(v), O = t.lastIndexOf(_), $ = N == -1 && O != -1 ? O : N != -1 && O == -1 ? N : N > O ? N : O, G == P && ($ = G), G != -1) {
                        for (R = ("" + e).split(m), D = R[0], A = R[1] || f, E = D.length, I = A.length, T && e * -1 >= 0 && (T = !1), e = t.substring(0, G), T && !L && (e += "-"), F = G; F < P; F++) {
                            if (z = t.charAt(F), H == -1) {
                                if ($ - F < E) {
                                    e += D;
                                    break
                                }
                            } else if (O != -1 && O < F && (M = f), H - F <= E && H - F > -1 && (e += D, F = H), H === F) {
                                e += (A ? s : f) + A, F += $ - H + 1;
                                continue
                            }
                            z === _ ? (e += z, M = z) : z === v && (e += M)
                        }
                        if (B && (e = a(e, G + (T && !L ? 1 : 0), Math.max($, E + G), r)), $ >= G && (e += t.substring($ + 1)), k || x) {
                            for (R = f, F = 0, P = e.length; F < P; F++) z = e.charAt(F), R += "$" === z || "%" === z ? y : z;
                            e = R
                        }
                        if (P = w.length)
                            for (F = 0; F < P; F++) e = e.replace(b, w[F])
                    }
                    return e
                }
                var a, l, c, d = /dddd|ddd|dd|d|MMMM|MMM|MM|M|yyyy|yy|HH|H|hh|h|mm|m|fff|ff|f|tt|ss|s|zzz|zz|z|"[^"]*"|'[^']*'/g,
                    u = /^(n|c|p|e)(\d*)$/i,
                    h = /(\\.)|(['][^']*[']?)|(["][^"]*["]?)/g,
                    p = /\,/g,
                    f = "",
                    m = ".",
                    g = ",",
                    v = "#",
                    _ = "0",
                    b = "??",
                    w = "en-US",
                    y = {}.toString;
                ve.cultures["en-US"] = {
                    name: w,
                    numberFormat: {
                        pattern: ["-n"],
                        decimals: 2,
                        ",": ",",
                        ".": ".",
                        groupSize: [3],
                        percent: {
                            pattern: ["-n %", "n %"],
                            decimals: 2,
                            ",": ",",
                            ".": ".",
                            groupSize: [3],
                            symbol: "%"
                        },
                        currency: {
                            name: "US Dollar",
                            abbr: "USD",
                            pattern: ["($n)", "$n"],
                            decimals: 2,
                            ",": ",",
                            ".": ".",
                            groupSize: [3],
                            symbol: "$"
                        }
                    },
                    calendars: {
                        standard: {
                            days: {
                                names: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"],
                                namesAbbr: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"],
                                namesShort: ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"]
                            },
                            months: {
                                names: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
                                namesAbbr: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"]
                            },
                            AM: ["AM", "am", "AM"],
                            PM: ["PM", "pm", "PM"],
                            patterns: {
                                d: "M/d/yyyy",
                                D: "dddd, MMMM dd, yyyy",
                                F: "dddd, MMMM dd, yyyy h:mm:ss tt",
                                g: "M/d/yyyy h:mm tt",
                                G: "M/d/yyyy h:mm:ss tt",
                                m: "MMMM dd",
                                M: "MMMM dd",
                                s: "yyyy'-'MM'-'ddTHH':'mm':'ss",
                                t: "h:mm tt",
                                T: "h:mm:ss tt",
                                u: "yyyy'-'MM'-'dd HH':'mm':'ss'Z'",
                                y: "MMMM, yyyy",
                                Y: "MMMM, yyyy"
                            },
                            "/": "/",
                            ":": ":",
                            firstDay: 0,
                            twoDigitYearMax: 2029
                        }
                    }
                }, ve.culture = function(e) {
                    var i, o = ve.cultures;
                    return e === n ? o.current : (i = t(e) || o[w], i.calendar = i.calendars.standard, o.current = i, n)
                }, ve.findCulture = t, ve.getCulture = i, ve.culture(w), a = function(e, t, i, o) {
                    var r, s, a, l, c, d, u = e.indexOf(o[m]),
                        h = o.groupSize.slice(),
                        p = h.shift();
                    if (i = u !== -1 ? u : i + 1, r = e.substring(t, i), s = r.length, s >= p) {
                        for (a = s, l = []; a > -1;)
                            if (c = r.substring(a - p, a), c && l.push(c), a -= p, d = h.shift(), p = d !== n ? d : p, 0 === p) {
                                a > 0 && l.push(r.substring(0, a));
                                break
                            }
                        r = l.reverse().join(o[g]), e = e.substring(0, t) + r + e.substring(i)
                    }
                    return e
                }, l = function(e, t, n) {
                    return t = t || 0, e = ("" + e).split("e"), e = Math.round(+(e[0] + "e" + (e[1] ? +e[1] + t : t))), n && (e = -e), e = ("" + e).split("e"), e = +(e[0] + "e" + (e[1] ? +e[1] - t : -t)), e.toFixed(Math.min(t, 20))
                }, c = function(e, t, i) {
                    if (t) {
                        if ("[object Date]" === y.call(e)) return o(e, t, i);
                        if (typeof e === Re) return s(e, t, i)
                    }
                    return e !== n ? e : ""
                }, ve.format = function(e) {
                    var t = arguments;
                    return e.replace(De, function(e, n, i) {
                        var o = t[parseInt(n, 10) + 1];
                        return c(o, i ? i.substring(1) : "")
                    })
                }, ve._extractFormat = function(e) {
                    return "{0:" === e.slice(0, 3) && (e = e.slice(3, e.length - 1)), e
                }, ve._activeElement = function() {
                    try {
                        return document.activeElement
                    } catch (e) {
                        return document.documentElement.activeElement
                    }
                }, ve._round = l, ve._outerWidth = function(t, n) {
                    return e(t).outerWidth(n || !1) || 0
                }, ve._outerHeight = function(t, n) {
                    return e(t).outerHeight(n || !1) || 0
                }, ve.toString = c
            }(),
            function() {
                function t(e, t, n) {
                    return !(e >= t && e <= n)
                }

                function i(e) {
                    return e.charAt(0)
                }

                function o(t) {
                    return e.map(t, i)
                }

                function r(e, t) {
                    t || 23 !== e.getHours() || e.setHours(e.getHours() + 2)
                }

                function s(e) {
                    for (var t = 0, n = e.length, i = []; t < n; t++) i[t] = (e[t] + "").toLowerCase();
                    return i
                }

                function a(e) {
                    var t, n = {};
                    for (t in e) n[t] = s(e[t]);
                    return n
                }

                function l(e, i, s, l) {
                    if (!e) return null;
                    var c, d, u, h, p, g, v, _, b, y, k, x, C, S = function(e) {
                            for (var t = 0; i[L] === e;) t++, L++;
                            return t > 0 && (L -= 1), t
                        },
                        T = function(t) {
                            var n = w[t] || RegExp("^\\d{1," + t + "}"),
                                i = e.substr(H, t).match(n);
                            return i ? (i = i[0], H += i.length, parseInt(i, 10)) : null
                        },
                        D = function(t, n) {
                            for (var i, o, r, s = 0, a = t.length, l = 0, c = 0; s < a; s++) i = t[s], o = i.length, r = e.substr(H, o), n && (r = r.toLowerCase()), r == i && o > l && (l = o, c = s);
                            return l ? (H += l, c + 1) : null
                        },
                        A = function() {
                            var t = !1;
                            return e.charAt(H) === i[L] && (H++, t = !0), t
                        },
                        E = s.calendars.standard,
                        I = null,
                        M = null,
                        R = null,
                        F = null,
                        P = null,
                        z = null,
                        B = null,
                        L = 0,
                        H = 0,
                        N = !1,
                        O = new Date,
                        V = E.twoDigitYearMax || 2029,
                        W = O.getFullYear();
                    for (i || (i = "d"), h = E.patterns[i], h && (i = h), i = i.split(""), u = i.length; L < u; L++)
                        if (c = i[L], N) "'" === c ? N = !1 : A();
                        else if ("d" === c) {
                        if (d = S("d"), E._lowerDays || (E._lowerDays = a(E.days)), null !== R && d > 2) continue;
                        if (R = d < 3 ? T(2) : D(E._lowerDays[3 == d ? "namesAbbr" : "names"], !0), null === R || t(R, 1, 31)) return null
                    } else if ("M" === c) {
                        if (d = S("M"), E._lowerMonths || (E._lowerMonths = a(E.months)), M = d < 3 ? T(2) : D(E._lowerMonths[3 == d ? "namesAbbr" : "names"], !0), null === M || t(M, 1, 12)) return null;
                        M -= 1
                    } else if ("y" === c) {
                        if (d = S("y"), I = T(d), null === I) return null;
                        2 == d && ("string" == typeof V && (V = W + parseInt(V, 10)), I = W - W % 100 + I, I > V && (I -= 100))
                    } else if ("h" === c) {
                        if (S("h"), F = T(2), 12 == F && (F = 0), null === F || t(F, 0, 11)) return null
                    } else if ("H" === c) {
                        if (S("H"), F = T(2), null === F || t(F, 0, 23)) return null
                    } else if ("m" === c) {
                        if (S("m"), P = T(2), null === P || t(P, 0, 59)) return null
                    } else if ("s" === c) {
                        if (S("s"), z = T(2), null === z || t(z, 0, 59)) return null
                    } else if ("f" === c) {
                        if (d = S("f"), C = e.substr(H, d).match(w[3]), B = T(d), null !== B && (B = parseFloat("0." + C[0], 10), B = ve._round(B, 3), B *= 1e3), null === B || t(B, 0, 999)) return null
                    } else if ("t" === c) {
                        if (d = S("t"), _ = E.AM, b = E.PM, 1 === d && (_ = o(_), b = o(b)), p = D(b), !p && !D(_)) return null
                    } else if ("z" === c) {
                        if (g = !0, d = S("z"), "Z" === e.substr(H, 1)) {
                            A();
                            continue
                        }
                        if (v = e.substr(H, 6).match(d > 2 ? m : f), !v) return null;
                        if (v = v[0].split(":"), y = v[0], k = v[1], !k && y.length > 3 && (H = y.length - 2, k = y.substring(H), y = y.substring(0, H)), y = parseInt(y, 10), t(y, -12, 13)) return null;
                        if (d > 2 && (k = v[0][0] + k, k = parseInt(k, 10), isNaN(k) || t(k, -59, 59))) return null
                    } else if ("'" === c) N = !0, A();
                    else if (!A()) return null;
                    return l && !/^\s*$/.test(e.substr(H)) ? null : (x = null !== F || null !== P || z || null, null === I && null === M && null === R && x ? (I = W, M = O.getMonth(), R = O.getDate()) : (null === I && (I = W), null === R && (R = 1)), p && F < 12 && (F += 12), g ? (y && (F += -y), k && (P += -k), e = new Date(Date.UTC(I, M, R, F, P, z, B))) : (e = new Date(I, M, R, F, P, z, B), r(e, F)), I < 100 && e.setFullYear(I), e.getDate() !== R && g === n ? null : e)
                }

                function c(e) {
                    var t = "-" === e.substr(0, 1) ? -1 : 1;
                    return e = e.substring(1), e = 60 * parseInt(e.substr(0, 2), 10) + parseInt(e.substring(2), 10), t * e
                }

                function d(e) {
                    var t, n, i, o = xe.max(_.length, b.length),
                        r = e.calendar || e.calendars.standard,
                        s = r.patterns,
                        a = [];
                    for (i = 0; i < o; i++) {
                        for (t = _[i], n = 0; n < t.length; n++) a.push(s[t[n]]);
                        a = a.concat(b[i])
                    }
                    return a
                }

                function u(e, t, n, i) {
                    var o, r, s, a;
                    if ("[object Date]" === y.call(e)) return e;
                    if (o = 0, r = null, e && 0 === e.indexOf("/D") && (r = g.exec(e))) return r = r[1], a = v.exec(r.substring(1)), r = new Date(parseInt(r, 10)), a && (a = c(a[0]), r = ve.timezone.apply(r, 0), r = ve.timezone.convert(r, 0, -1 * a)), r;
                    for (n = ve.getCulture(n), t || (t = d(n)), t = we(t) ? t : [t], s = t.length; o < s; o++)
                        if (r = l(e, t[o], n, i)) return r;
                    return r
                }
                var h = /\u00A0/g,
                    p = /[eE][\-+]?[0-9]+/,
                    f = /[+|\-]\d{1,2}/,
                    m = /[+|\-]\d{1,2}:?\d{2}/,
                    g = /^\/Date\((.*?)\)\/$/,
                    v = /[+-]\d*/,
                    _ = [
                        [],
                        ["G", "g", "F"],
                        ["D", "d", "y", "m", "T", "t"]
                    ],
                    b = [
                        ["yyyy-MM-ddTHH:mm:ss.fffffffzzz", "yyyy-MM-ddTHH:mm:ss.fffffff", "yyyy-MM-ddTHH:mm:ss.fffzzz", "yyyy-MM-ddTHH:mm:ss.fff", "ddd MMM dd yyyy HH:mm:ss", "yyyy-MM-ddTHH:mm:sszzz", "yyyy-MM-ddTHH:mmzzz", "yyyy-MM-ddTHH:mmzz", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-dd HH:mm:ss", "yyyy/MM/dd HH:mm:ss"],
                        ["yyyy-MM-ddTHH:mm", "yyyy-MM-dd HH:mm", "yyyy/MM/dd HH:mm"],
                        ["yyyy/MM/dd", "yyyy-MM-dd", "HH:mm:ss", "HH:mm"]
                    ],
                    w = {
                        2: /^\d{1,2}/,
                        3: /^\d{1,3}/,
                        4: /^\d{4}/
                    },
                    y = {}.toString;
                ve.parseDate = function(e, t, n) {
                    return u(e, t, n, !1)
                }, ve.parseExactDate = function(e, t, n) {
                    return u(e, t, n, !0)
                }, ve.parseInt = function(e, t) {
                    var n = ve.parseFloat(e, t);
                    return n && (n = 0 | n), n
                }, ve.parseFloat = function(e, t, n) {
                    if (!e && 0 !== e) return null;
                    if (typeof e === Re) return e;
                    e = "" + e, t = ve.getCulture(t);
                    var i, o, r = t.numberFormat,
                        s = r.percent,
                        a = r.currency,
                        l = a.symbol,
                        c = s.symbol,
                        d = e.indexOf("-");
                    return p.test(e) ? (e = parseFloat(e.replace(r["."], ".")), isNaN(e) && (e = null), e) : d > 0 ? null : (d = d > -1, e.indexOf(l) > -1 || n && n.toLowerCase().indexOf("c") > -1 ? (r = a, i = r.pattern[0].replace("$", l).split("n"), e.indexOf(i[0]) > -1 && e.indexOf(i[1]) > -1 && (e = e.replace(i[0], "").replace(i[1], ""), d = !0)) : e.indexOf(c) > -1 && (o = !0, r = s, l = c), e = e.replace("-", "").replace(l, "").replace(h, " ").split(r[","].replace(h, " ")).join("").replace(r["."], "."), e = parseFloat(e), isNaN(e) ? e = null : d && (e *= -1), e && o && (e /= 100), e)
                }
            }(),
            function() {
                var i, o, r, s, a, l, c, u, h, p;
                Se._scrollbar = n, Se.scrollbar = function(e) {
                    if (isNaN(Se._scrollbar) || e) {
                        var t, n = document.createElement("div");
                        return n.style.cssText = "overflow:scroll;overflow-x:hidden;zoom:1;clear:both;display:block", n.innerHTML = "&nbsp;", document.body.appendChild(n), Se._scrollbar = t = n.offsetWidth - n.scrollWidth, document.body.removeChild(n), t
                    }
                    return Se._scrollbar
                }, Se.isRtl = function(t) {
                    return e(t).closest(".k-rtl").length > 0
                }, i = document.createElement("table");
                try {
                    i.innerHTML = "<tr><td></td></tr>", Se.tbodyInnerHtml = !0
                } catch (f) {
                    Se.tbodyInnerHtml = !1
                }
                Se.touch = "ontouchstart" in t, o = document.documentElement.style, r = Se.transitions = !1, s = Se.transforms = !1, a = "HTMLElement" in t ? HTMLElement.prototype : [], Se.hasHW3D = "WebKitCSSMatrix" in t && "m11" in new t.WebKitCSSMatrix || "MozPerspective" in o || "msPerspective" in o, Se.cssFlexbox = "flexWrap" in o || "WebkitFlexWrap" in o || "msFlexWrap" in o, be(["Moz", "webkit", "O", "ms"], function() {
                    var e, t = "" + this,
                        n = typeof i.style[t + "Transition"] === Me;
                    if (n || typeof i.style[t + "Transform"] === Me) return e = t.toLowerCase(), s = {
                        css: "ms" != e ? "-" + e + "-" : "",
                        prefix: t,
                        event: "o" === e || "webkit" === e ? e : ""
                    }, n && (r = s, r.event = r.event ? r.event + "TransitionEnd" : "transitionend"), !1
                }), i = null, Se.transforms = s, Se.transitions = r, Se.devicePixelRatio = t.devicePixelRatio === n ? 1 : t.devicePixelRatio;
                try {
                    Se.screenWidth = t.outerWidth || t.screen ? t.screen.availWidth : t.innerWidth, Se.screenHeight = t.outerHeight || t.screen ? t.screen.availHeight : t.innerHeight
                } catch (f) {
                    Se.screenWidth = t.screen.availWidth, Se.screenHeight = t.screen.availHeight
                }
                Se.detectOS = function(e) {
                        var n, i, o = !1,
                            r = [],
                            s = !/mobile safari/i.test(e),
                            a = {
                                wp: /(Windows Phone(?: OS)?)\s(\d+)\.(\d+(\.\d+)?)/,
                                fire: /(Silk)\/(\d+)\.(\d+(\.\d+)?)/,
                                android: /(Android|Android.*(?:Opera|Firefox).*?\/)\s*(\d+)\.(\d+(\.\d+)?)/,
                                iphone: /(iPhone|iPod).*OS\s+(\d+)[\._]([\d\._]+)/,
                                ipad: /(iPad).*OS\s+(\d+)[\._]([\d_]+)/,
                                meego: /(MeeGo).+NokiaBrowser\/(\d+)\.([\d\._]+)/,
                                webos: /(webOS)\/(\d+)\.(\d+(\.\d+)?)/,
                                blackberry: /(BlackBerry|BB10).*?Version\/(\d+)\.(\d+(\.\d+)?)/,
                                playbook: /(PlayBook).*?Tablet\s*OS\s*(\d+)\.(\d+(\.\d+)?)/,
                                windows: /(MSIE)\s+(\d+)\.(\d+(\.\d+)?)/,
                                tizen: /(tizen).*?Version\/(\d+)\.(\d+(\.\d+)?)/i,
                                sailfish: /(sailfish).*rv:(\d+)\.(\d+(\.\d+)?).*firefox/i,
                                ffos: /(Mobile).*rv:(\d+)\.(\d+(\.\d+)?).*Firefox/
                            },
                            l = {
                                ios: /^i(phone|pad|pod)$/i,
                                android: /^android|fire$/i,
                                blackberry: /^blackberry|playbook/i,
                                windows: /windows/,
                                wp: /wp/,
                                flat: /sailfish|ffos|tizen/i,
                                meego: /meego/
                            },
                            c = {
                                tablet: /playbook|ipad|fire/i
                            },
                            u = {
                                omini: /Opera\sMini/i,
                                omobile: /Opera\sMobi/i,
                                firefox: /Firefox|Fennec/i,
                                mobilesafari: /version\/.*safari/i,
                                ie: /MSIE|Windows\sPhone/i,
                                chrome: /chrome|crios/i,
                                webkit: /webkit/i
                            };
                        for (i in a)
                            if (a.hasOwnProperty(i) && (r = e.match(a[i]))) {
                                if ("windows" == i && "plugins" in navigator) return !1;
                                o = {}, o.device = i, o.tablet = d(i, c, !1), o.browser = d(e, u, "default"), o.name = d(i, l), o[o.name] = !0, o.majorVersion = r[2], o.minorVersion = r[3].replace("_", "."), n = o.minorVersion.replace(".", "").substr(0, 2), o.flatVersion = o.majorVersion + n + Array(3 - (n.length < 3 ? n.length : 2)).join("0"), o.cordova = typeof t.PhoneGap !== Be || typeof t.cordova !== Be, o.appMode = t.navigator.standalone || /file|local|wmapp/.test(t.location.protocol) || o.cordova, o.android && (Se.devicePixelRatio < 1.5 && o.flatVersion < 400 || s) && (Se.screenWidth > 800 || Se.screenHeight > 800) && (o.tablet = i);
                                break
                            }
                        return o
                    }, l = Se.mobileOS = Se.detectOS(navigator.userAgent), Se.wpDevicePixelRatio = l.wp ? screen.width / 320 : 0, Se.hasNativeScrolling = !1, (l.ios || l.android && l.majorVersion > 2 || l.wp) && (Se.hasNativeScrolling = l), Se.delayedClick = function() {
                        if (Se.touch) {
                            if (l.ios) return !0;
                            if (l.android) return !Se.browser.chrome || !(Se.browser.version < 32) && !(e("meta[name=viewport]").attr("content") || "").match(/user-scalable=no/i)
                        }
                        return !1
                    }, Se.mouseAndTouchPresent = Se.touch && !(Se.mobileOS.ios || Se.mobileOS.android), Se.detectBrowser = function(e) {
                        var t, n = !1,
                            i = [],
                            o = {
                                edge: /(edge)[ \/]([\w.]+)/i,
                                webkit: /(chrome|crios)[ \/]([\w.]+)/i,
                                safari: /(webkit)[ \/]([\w.]+)/i,
                                opera: /(opera)(?:.*version|)[ \/]([\w.]+)/i,
                                msie: /(msie\s|trident.*? rv:)([\w.]+)/i,
                                mozilla: /(mozilla)(?:.*? rv:([\w.]+)|)/i
                            };
                        for (t in o)
                            if (o.hasOwnProperty(t) && (i = e.match(o[t]))) {
                                n = {}, n[t] = !0, n[i[1].toLowerCase().split(" ")[0].split("/")[0]] = !0, n.version = parseInt(document.documentMode || i[2], 10);
                                break
                            }
                        return n
                    }, Se.browser = Se.detectBrowser(navigator.userAgent), Se.detectClipboardAccess = function() {
                        var e = {
                            copy: !!document.queryCommandSupported && document.queryCommandSupported("copy"),
                            cut: !!document.queryCommandSupported && document.queryCommandSupported("cut"),
                            paste: !!document.queryCommandSupported && document.queryCommandSupported("paste")
                        };
                        return Se.browser.chrome && (e.paste = !1, Se.browser.version >= 43 && (e.copy = !0, e.cut = !0)), e
                    }, Se.clipboard = Se.detectClipboardAccess(), Se.zoomLevel = function() {
                        var e, n, i;
                        try {
                            return e = Se.browser, n = 0, i = document.documentElement, e.msie && 11 == e.version && i.scrollHeight > i.clientHeight && !Se.touch && (n = Se.scrollbar()), Se.touch ? i.clientWidth / t.innerWidth : e.msie && e.version >= 10 ? ((top || t).document.documentElement.offsetWidth + n) / (top || t).innerWidth : 1
                        } catch (o) {
                            return 1
                        }
                    }, Se.cssBorderSpacing = n !== o.borderSpacing && !(Se.browser.msie && Se.browser.version < 8),
                    function(t) {
                        var n = "",
                            i = e(document.documentElement),
                            o = parseInt(t.version, 10);
                        t.msie ? n = "ie" : t.mozilla ? n = "ff" : t.safari ? n = "safari" : t.webkit ? n = "webkit" : t.opera ? n = "opera" : t.edge && (n = "edge"), n && (n = "k-" + n + " k-" + n + o), Se.mobileOS && (n += " k-mobile"), Se.cssFlexbox || (n += " k-no-flexbox"), i.addClass(n)
                    }(Se.browser), Se.eventCapture = document.documentElement.addEventListener, c = document.createElement("input"), Se.placeholder = "placeholder" in c, Se.propertyChangeEvent = "onpropertychange" in c, Se.input = function() {
                        for (var e, t = ["number", "date", "time", "month", "week", "datetime", "datetime-local"], n = t.length, i = "test", o = {}, r = 0; r < n; r++) e = t[r], c.setAttribute("type", e), c.value = i, o[e.replace("-", "")] = "text" !== c.type && c.value !== i;
                        return o
                    }(), c.style.cssText = "float:left;", Se.cssFloat = !!c.style.cssFloat, c = null, Se.stableSort = function() {
                        var e, t = 513,
                            n = [{
                                index: 0,
                                field: "b"
                            }];
                        for (e = 1; e < t; e++) n.push({
                            index: e,
                            field: "a"
                        });
                        return n.sort(function(e, t) {
                            return e.field > t.field ? 1 : e.field < t.field ? -1 : 0
                        }), 1 === n[0].index
                    }(), Se.matchesSelector = a.webkitMatchesSelector || a.mozMatchesSelector || a.msMatchesSelector || a.oMatchesSelector || a.matchesSelector || a.matches || function(t) {
                        for (var n = document.querySelectorAll ? (this.parentNode || document).querySelectorAll(t) || [] : e(t), i = n.length; i--;)
                            if (n[i] == this) return !0;
                        return !1
                    }, Se.matchMedia = "matchMedia" in t, Se.pushState = t.history && t.history.pushState, u = document.documentMode, Se.hashChange = "onhashchange" in t && !(Se.browser.msie && (!u || u <= 8)), Se.customElements = "registerElement" in t.document, h = Se.browser.chrome, p = Se.browser.mozilla, Se.msPointers = !h && t.MSPointerEvent, Se.pointers = !h && !p && t.PointerEvent, Se.kineticScrollNeeded = l && (Se.touch || Se.msPointers || Se.pointers)
            }(), U = {
                left: {
                    reverse: "right"
                },
                right: {
                    reverse: "left"
                },
                down: {
                    reverse: "up"
                },
                up: {
                    reverse: "down"
                },
                top: {
                    reverse: "bottom"
                },
                bottom: {
                    reverse: "top"
                },
                "in": {
                    reverse: "out"
                },
                out: {
                    reverse: "in"
                }
            }, q = {}, e.extend(q, {
                enabled: !0,
                Element: function(t) {
                    this.element = e(t)
                },
                promise: function(e, t) {
                    e.is(":visible") || e.css({
                        display: e.data("olddisplay") || "block"
                    }).css("display"), t.hide && e.data("olddisplay", e.css("display")).hide(), t.init && t.init(), t.completeCallback && t.completeCallback(e), e.dequeue()
                },
                disable: function() {
                    this.enabled = !1, this.promise = this.promiseShim
                },
                enable: function() {
                    this.enabled = !0, this.promise = this.animatedPromise
                }
            }), q.promiseShim = q.promise, "kendoAnimate" in e.fn || _e(e.fn, {
                kendoStop: function(e, t) {
                    return this.stop(e, t)
                },
                kendoAnimate: function(e, t, n, i) {
                    return y(this, e, t, n, i)
                },
                kendoAddClass: function(e, t) {
                    return ve.toggleClass(this, e, t, !0)
                },
                kendoRemoveClass: function(e, t) {
                    return ve.toggleClass(this, e, t, !1)
                },
                kendoToggleClass: function(e, t, n) {
                    return ve.toggleClass(this, e, t, n)
                }
            }), j = /&/g, G = /</g, $ = /"/g, K = /'/g, Y = />/g, Q = function(e) {
                return e.target
            }, Se.touch && (Q = function(e) {
                var t = "originalEvent" in e ? e.originalEvent.changedTouches : "changedTouches" in e ? e.changedTouches : null;
                return t ? document.elementFromPoint(t[0].clientX, t[0].clientY) : e.target
            }, be(["swipe", "swipeLeft", "swipeRight", "swipeUp", "swipeDown", "doubleTap", "tap"], function(t, n) {
                e.fn[n] = function(e) {
                    return this.bind(n, e)
                }
            })), Se.touch ? Se.mobileOS ? (Se.mousedown = "touchstart", Se.mouseup = "touchend", Se.mousemove = "touchmove", Se.mousecancel = "touchcancel", Se.click = "touchend", Se.resize = "orientationchange") : (Se.mousedown = "mousedown touchstart", Se.mouseup = "mouseup touchend", Se.mousemove = "mousemove touchmove", Se.mousecancel = "mouseleave touchcancel", Se.click = "click", Se.resize = "resize") : Se.pointers ? (Se.mousemove = "pointermove", Se.mousedown = "pointerdown", Se.mouseup = "pointerup", Se.mousecancel = "pointercancel", Se.click = "pointerup", Se.resize = "orientationchange resize") : Se.msPointers ? (Se.mousemove = "MSPointerMove", Se.mousedown = "MSPointerDown", Se.mouseup = "MSPointerUp", Se.mousecancel = "MSPointerCancel", Se.click = "MSPointerUp", Se.resize = "orientationchange resize") : (Se.mousemove = "mousemove", Se.mousedown = "mousedown", Se.mouseup = "mouseup", Se.mousecancel = "mouseleave", Se.click = "click", Se.resize = "resize"), X = function(e, t) {
                var n, i, o, r, s = t || "d",
                    a = 1;
                for (i = 0, o = e.length; i < o; i++) r = e[i], "" !== r && (n = r.indexOf("["), 0 !== n && (n == -1 ? r = "." + r : (a++, r = "." + r.substring(0, n) + " || {})" + r.substring(n))), a++, s += r + (i < o - 1 ? " || {})" : ")"));
                return Array(a).join("(") + s
            }, Z = /^([a-z]+:)?\/\//i, _e(ve, {
                widgets: [],
                _widgetRegisteredCallbacks: [],
                ui: ve.ui || {},
                fx: ve.fx || b,
                effects: ve.effects || q,
                mobile: ve.mobile || {},
                data: ve.data || {},
                dataviz: ve.dataviz || {},
                drawing: ve.drawing || {},
                spreadsheet: {
                    messages: {}
                },
                keys: {
                    INSERT: 45,
                    DELETE: 46,
                    BACKSPACE: 8,
                    TAB: 9,
                    ENTER: 13,
                    ESC: 27,
                    LEFT: 37,
                    UP: 38,
                    RIGHT: 39,
                    DOWN: 40,
                    END: 35,
                    HOME: 36,
                    SPACEBAR: 32,
                    PAGEUP: 33,
                    PAGEDOWN: 34,
                    F2: 113,
                    F10: 121,
                    F12: 123,
                    NUMPAD_PLUS: 107,
                    NUMPAD_MINUS: 109,
                    NUMPAD_DOT: 110
                },
                support: ve.support || Se,
                animate: ve.animate || y,
                ns: "",
                attr: function(e) {
                    return "data-" + ve.ns + e
                },
                getShadows: s,
                wrap: a,
                deepExtend: l,
                getComputedStyles: p,
                webComponents: ve.webComponents || [],
                isScrollable: f,
                scrollLeft: m,
                size: g,
                toCamelCase: h,
                toHyphens: u,
                getOffset: ve.getOffset || v,
                parseEffects: ve.parseEffects || _,
                toggleClass: ve.toggleClass || k,
                directions: ve.directions || U,
                Observable: z,
                Class: i,
                Template: M,
                template: ye(M.compile, M),
                render: ye(M.render, M),
                stringify: ye(Ce.stringify, Ce),
                eventTarget: Q,
                htmlEncode: x,
                isLocalUrl: function(e) {
                    return e && !Z.test(e)
                },
                expr: function(e, t, n) {
                    return e = e || "", typeof t == Me && (n = t, t = !1), n = n || "d", e && "[" !== e.charAt(0) && (e = "." + e), t ? (e = e.replace(/"([^.]*)\.([^"]*)"/g, '"$1_$DOT$_$2"'), e = e.replace(/'([^.]*)\.([^']*)'/g, "'$1_$DOT$_$2'"), e = X(e.split("."), n), e = e.replace(/_\$DOT\$_/g, ".")) : e = n + e, e
                },
                getter: function(e, t) {
                    var n = e + t;
                    return Le[n] = Le[n] || Function("d", "return " + ve.expr(e, t))
                },
                setter: function(e) {
                    return He[e] = He[e] || Function("d,value", ve.expr(e) + "=value")
                },
                accessor: function(e) {
                    return {
                        get: ve.getter(e),
                        set: ve.setter(e)
                    }
                },
                guid: function() {
                    var e, t, n = "";
                    for (e = 0; e < 32; e++) t = 16 * xe.random() | 0, 8 != e && 12 != e && 16 != e && 20 != e || (n += "-"), n += (12 == e ? 4 : 16 == e ? 3 & t | 8 : t).toString(16);
                    return n
                },
                roleSelector: function(e) {
                    return e.replace(/(\S+)/g, "[" + ve.attr("role") + "=$1],").slice(0, -1)
                },
                directiveSelector: function(e) {
                    var t, n = e.split(" ");
                    if (n)
                        for (t = 0; t < n.length; t++) "view" != n[t] && (n[t] = n[t].replace(/(\w*)(view|bar|strip|over)$/, "$1-$2"));
                    return n.join(" ").replace(/(\S+)/g, "kendo-mobile-$1,").slice(0, -1);
                },
                triggeredByInput: function(e) {
                    return /^(label|input|textarea|select)$/i.test(e.target.tagName)
                },
                onWidgetRegistered: function(e) {
                    for (var t = 0, n = ve.widgets.length; t < n; t++) e(ve.widgets[t]);
                    ve._widgetRegisteredCallbacks.push(e)
                },
                logToConsole: function(e, i) {
                    var o = t.console;
                    !ve.suppressLog && n !== o && o.log && o[i || "log"](e)
                }
            }), J = z.extend({
                init: function(e, t) {
                    var n, i = this;
                    i.element = ve.jQuery(e).handler(i), i.angular("init", t), z.fn.init.call(i), n = t ? t.dataSource : null, n && (t = _e({}, t, {
                        dataSource: {}
                    })), t = i.options = _e(!0, {}, i.options, t), n && (t.dataSource = n), i.element.attr(ve.attr("role")) || i.element.attr(ve.attr("role"), (t.name || "").toLowerCase()), i.element.data("kendo" + t.prefix + t.name, i), i.bind(i.events, t)
                },
                events: [],
                options: {
                    prefix: ""
                },
                _hasBindingTarget: function() {
                    return !!this.element[0].kendoBindingTarget
                },
                _tabindex: function(e) {
                    e = e || this.wrapper;
                    var t = this.element,
                        n = "tabindex",
                        i = e.attr(n) || t.attr(n);
                    t.removeAttr(n), e.attr(n, isNaN(i) ? 0 : i)
                },
                setOptions: function(t) {
                    this._setEvents(t), e.extend(this.options, t)
                },
                _setEvents: function(e) {
                    for (var t, n = this, i = 0, o = n.events.length; i < o; i++) t = n.events[i], n.options[t] && e[t] && n.unbind(t, n.options[t]);
                    n.bind(n.events, e)
                },
                resize: function(e) {
                    var t = this.getSize(),
                        n = this._size;
                    (e || (t.width > 0 || t.height > 0) && (!n || t.width !== n.width || t.height !== n.height)) && (this._size = t, this._resize(t, e), this.trigger("resize", t))
                },
                getSize: function() {
                    return ve.dimensions(this.element)
                },
                size: function(e) {
                    return e ? (this.setSize(e), n) : this.getSize()
                },
                setSize: e.noop,
                _resize: e.noop,
                destroy: function() {
                    var e = this;
                    e.element.removeData("kendo" + e.options.prefix + e.options.name), e.element.removeData("handler"), e.unbind()
                },
                _destroy: function() {
                    this.destroy()
                },
                angular: function() {},
                _muteAngularRebind: function(e) {
                    this._muteRebind = !0, e.call(this), this._muteRebind = !1
                }
            }), ee = J.extend({
                dataItems: function() {
                    return this.dataSource.flatView()
                },
                _angularItems: function(t) {
                    var n = this;
                    n.angular(t, function() {
                        return {
                            elements: n.items(),
                            data: e.map(n.dataItems(), function(e) {
                                return {
                                    dataItem: e
                                }
                            })
                        }
                    })
                }
            }), ve.dimensions = function(e, t) {
                var n = e[0];
                return t && e.css(t), {
                    width: n.offsetWidth,
                    height: n.offsetHeight
                }
            }, ve.notify = ke, te = /template$/i, ne = /^\s*(?:\{(?:.|\r\n|\n)*\}|\[(?:.|\r\n|\n)*\])\s*$/, ie = /^\{(\d+)(:[^\}]+)?\}|^\[[A-Za-z_]+\]$/, oe = /([A-Z])/g, ve.initWidget = function(i, o, r) {
                var s, a, l, c, d, u, h, p, f, m, g, v, _;
                if (r ? r.roles && (r = r.roles) : r = ve.ui.roles, i = i.nodeType ? i : i[0], u = i.getAttribute("data-" + ve.ns + "role")) {
                    f = u.indexOf(".") === -1, l = f ? r[u] : ve.getter(u)(t), g = e(i).data(), v = l ? "kendo" + l.fn.options.prefix + l.fn.options.name : "", m = f ? RegExp("^kendo.*" + u + "$", "i") : RegExp("^" + v + "$", "i");
                    for (_ in g)
                        if (_.match(m)) {
                            if (_ !== v) return g[_];
                            s = g[_]
                        }
                    if (l) {
                        for (p = C(i, "dataSource"), o = e.extend({}, S(i, l.fn.options), o), p && (o.dataSource = typeof p === Me ? ve.getter(p)(t) : p), c = 0, d = l.fn.events.length; c < d; c++) a = l.fn.events[c], h = C(i, a), h !== n && (o[a] = ve.getter(h)(t));
                        return s ? e.isEmptyObject(o) || s.setOptions(o) : s = new l(i, o), s
                    }
                }
            }, ve.rolesFromNamespaces = function(e) {
                var t, n, i = [];
                for (e[0] || (e = [ve.ui, ve.dataviz.ui]), t = 0, n = e.length; t < n; t++) i[t] = e[t].roles;
                return _e.apply(null, [{}].concat(i.reverse()))
            }, ve.init = function(t) {
                var n = ve.rolesFromNamespaces(Ne.call(arguments, 1));
                e(t).find("[data-" + ve.ns + "role]").addBack().each(function() {
                    ve.initWidget(this, {}, n)
                })
            }, ve.destroy = function(t) {
                e(t).find("[data-" + ve.ns + "role]").addBack().each(function() {
                    var t, n = e(this).data();
                    for (t in n) 0 === t.indexOf("kendo") && typeof n[t].destroy === Ie && n[t].destroy()
                })
            }, ve.resize = function(t, n) {
                var i, o = e(t).find("[data-" + ve.ns + "role]").addBack().filter(D);
                o.length && (i = e.makeArray(o), i.sort(T), e.each(i, function() {
                    var t = ve.widgetInstance(e(this));
                    t && t.resize(n)
                }))
            }, ve.parseOptions = S, _e(ve.ui, {
                Widget: J,
                DataBoundWidget: ee,
                roles: {},
                progress: function(t, n, i) {
                    var o, r, s, a, l, c = t.find(".k-loading-mask"),
                        d = ve.support,
                        u = d.browser;
                    i = e.extend({}, {
                        width: "100%",
                        height: "100%",
                        top: t.scrollTop(),
                        opacity: !1
                    }, i), l = i.opacity ? "k-loading-mask k-opaque" : "k-loading-mask", n ? c.length || (o = d.isRtl(t), r = o ? "right" : "left", a = t.scrollLeft(), s = u.webkit && o ? t[0].scrollWidth - t.width() - 2 * a : 0, c = e(ve.format("<div class='{0}'><span class='k-loading-text'>{1}</span><div class='k-loading-image'/><div class='k-loading-color'/></div>", l, ve.ui.progress.messages.loading)).width(i.width).height(i.height).css("top", i.top).css(r, Math.abs(a) + s).prependTo(t)) : c && c.remove()
                },
                plugin: function(t, i, o) {
                    var r, s, a, l, c = t.fn.options.name;
                    for (i = i || ve.ui, o = o || "", i[c] = t, i.roles[c.toLowerCase()] = t, r = "getKendo" + o + c, c = "kendo" + o + c, s = {
                            name: c,
                            widget: t,
                            prefix: o || ""
                        }, ve.widgets.push(s), a = 0, l = ve._widgetRegisteredCallbacks.length; a < l; a++) ve._widgetRegisteredCallbacks[a](s);
                    e.fn[c] = function(i) {
                        var o, r = this;
                        return typeof i === Me ? (o = Ne.call(arguments, 1), this.each(function() {
                            var t, s, a = e.data(this, c);
                            if (!a) throw Error(ve.format("Cannot call method '{0}' of {1} before it is initialized", i, c));
                            if (t = a[i], typeof t !== Ie) throw Error(ve.format("Cannot find method '{0}' of {1}", i, c));
                            if (s = t.apply(a, o), s !== n) return r = s, !1
                        })) : this.each(function() {
                            return new t(this, i)
                        }), r
                    }, e.fn[c].widget = t, e.fn[r] = function() {
                        return this.data(c)
                    }
                }
            }), ve.ui.progress.messages = {
                loading: "Loading..."
            }, re = {
                bind: function() {
                    return this
                },
                nullObject: !0,
                options: {}
            }, se = J.extend({
                init: function(e, t) {
                    J.fn.init.call(this, e, t), this.element.autoApplyNS(), this.wrapper = this.element, this.element.addClass("km-widget")
                },
                destroy: function() {
                    J.fn.destroy.call(this), this.element.kendoDestroy()
                },
                options: {
                    prefix: "Mobile"
                },
                events: [],
                view: function() {
                    var e = this.element.closest(ve.roleSelector("view splitview modalview drawer"));
                    return ve.widgetInstance(e, ve.mobile.ui) || re
                },
                viewHasNativeScrolling: function() {
                    var e = this.view();
                    return e && e.options.useNativeScrolling
                },
                container: function() {
                    var e = this.element.closest(ve.roleSelector("view layout modalview drawer splitview"));
                    return ve.widgetInstance(e.eq(0), ve.mobile.ui) || re
                }
            }), _e(ve.mobile, {
                init: function(e) {
                    ve.init(e, ve.mobile.ui, ve.ui, ve.dataviz.ui)
                },
                appLevelNativeScrolling: function() {
                    return ve.mobile.application && ve.mobile.application.options && ve.mobile.application.options.useNativeScrolling
                },
                roles: {},
                ui: {
                    Widget: se,
                    DataBoundWidget: ee.extend(se.prototype),
                    roles: {},
                    plugin: function(e) {
                        ve.ui.plugin(e, ve.mobile.ui, "Mobile")
                    }
                }
            }), l(ve.dataviz, {
                init: function(e) {
                    ve.init(e, ve.dataviz.ui)
                },
                ui: {
                    roles: {},
                    themes: {},
                    views: [],
                    plugin: function(e) {
                        ve.ui.plugin(e, ve.dataviz.ui)
                    }
                },
                roles: {}
            }), ve.touchScroller = function(t, n) {
                return n || (n = {}), n.useNative = !0, e(t).map(function(t, i) {
                    return i = e(i), !(!Se.kineticScrollNeeded || !ve.mobile.ui.Scroller || i.data("kendoMobileScroller")) && (i.kendoMobileScroller(n), i.data("kendoMobileScroller"))
                })[0]
            }, ve.preventDefault = function(e) {
                e.preventDefault()
            }, ve.widgetInstance = function(e, n) {
                var i, o, r, s, a, l = e.data(ve.ns + "role"),
                    c = [];
                if (l) {
                    if ("content" === l && (l = "scroller"), "editortoolbar" === l && (r = e.data("kendoEditorToolbar"))) return r;
                    if (n)
                        if (n[0])
                            for (i = 0, o = n.length; i < o; i++) c.push(n[i].roles[l]);
                        else c.push(n.roles[l]);
                    else c = [ve.ui.roles[l], ve.dataviz.ui.roles[l], ve.mobile.ui.roles[l]];
                    for (l.indexOf(".") >= 0 && (c = [ve.getter(l)(t)]), i = 0, o = c.length; i < o; i++)
                        if (s = c[i], s && (a = e.data("kendo" + s.fn.options.prefix + s.fn.options.name))) return a
                }
            }, ve.onResize = function(n) {
                var i = n;
                return Se.mobileOS.android && (i = function() {
                    setTimeout(n, 600)
                }), e(t).on(Se.resize, i), i
            }, ve.unbindResize = function(n) {
                e(t).off(Se.resize, n)
            }, ve.attrValue = function(e, t) {
                return e.data(ve.ns + t)
            }, ve.days = {
                Sunday: 0,
                Monday: 1,
                Tuesday: 2,
                Wednesday: 3,
                Thursday: 4,
                Friday: 5,
                Saturday: 6
            }, e.extend(e.expr[":"], {
                kendoFocusable: function(t) {
                    var n = e.attr(t, "tabindex");
                    return A(t, !isNaN(n) && n > -1)
                }
            }), ae = ["mousedown", "mousemove", "mouseenter", "mouseleave", "mouseover", "mouseout", "mouseup", "click"], le = "label, input, [data-rel=external]", ce = {
                setupMouseMute: function() {
                    var t, n = 0,
                        i = ae.length,
                        o = document.documentElement;
                    if (!ce.mouseTrap && Se.eventCapture)
                        for (ce.mouseTrap = !0, ce.bustClick = !1, ce.captureMouse = !1, t = function(t) {
                                ce.captureMouse && ("click" === t.type ? ce.bustClick && !e(t.target).is(le) && (t.preventDefault(), t.stopPropagation()) : t.stopPropagation())
                            }; n < i; n++) o.addEventListener(ae[n], t, !0)
                },
                muteMouse: function(e) {
                    ce.captureMouse = !0, e.data.bustClick && (ce.bustClick = !0), clearTimeout(ce.mouseTrapTimeoutID)
                },
                unMuteMouse: function() {
                    clearTimeout(ce.mouseTrapTimeoutID), ce.mouseTrapTimeoutID = setTimeout(function() {
                        ce.captureMouse = !1, ce.bustClick = !1
                    }, 400)
                }
            }, de = {
                down: "touchstart mousedown",
                move: "mousemove touchmove",
                up: "mouseup touchend touchcancel",
                cancel: "mouseleave touchcancel"
            }, Se.touch && (Se.mobileOS.ios || Se.mobileOS.android) ? de = {
                down: "touchstart",
                move: "touchmove",
                up: "touchend touchcancel",
                cancel: "touchcancel"
            } : Se.pointers ? de = {
                down: "pointerdown",
                move: "pointermove",
                up: "pointerup",
                cancel: "pointercancel pointerleave"
            } : Se.msPointers && (de = {
                down: "MSPointerDown",
                move: "MSPointerMove",
                up: "MSPointerUp",
                cancel: "MSPointerCancel MSPointerLeave"
            }), !Se.msPointers || "onmspointerenter" in t || e.each({
                MSPointerEnter: "MSPointerOver",
                MSPointerLeave: "MSPointerOut"
            }, function(t, n) {
                e.event.special[t] = {
                    delegateType: n,
                    bindType: n,
                    handle: function(t) {
                        var i, o = this,
                            r = t.relatedTarget,
                            s = t.handleObj;
                        return r && (r === o || e.contains(o, r)) || (t.type = s.origType, i = s.handler.apply(this, arguments), t.type = n), i
                    }
                }
            }), ue = function(e) {
                return de[e] || e
            }, he = /([^ ]+)/g, ve.applyEventMap = function(e, t) {
                return e = e.replace(he, ue), t && (e = e.replace(he, "$1." + t)), e
            }, pe = e.fn.on, _e(!0, I, e), I.fn = I.prototype = new e, I.fn.constructor = I, I.fn.init = function(t, n) {
                return n && n instanceof e && !(n instanceof I) && (n = I(n)), e.fn.init.call(this, t, n, fe)
            }, I.fn.init.prototype = I.fn, fe = I(document), _e(I.fn, {
                handler: function(e) {
                    return this.data("handler", e), this
                },
                autoApplyNS: function(e) {
                    return this.data("kendoNS", e || ve.guid()), this
                },
                on: function() {
                    var e, t, n, i, o, r, s = this,
                        a = s.data("kendoNS");
                    return 1 === arguments.length ? pe.call(s, arguments[0]) : (e = s, t = Ne.call(arguments), typeof t[t.length - 1] === Be && t.pop(), n = t[t.length - 1], i = ve.applyEventMap(t[0], a), Se.mouseAndTouchPresent && i.search(/mouse|click/) > -1 && this[0] !== document.documentElement && (ce.setupMouseMute(), o = 2 === t.length ? null : t[1], r = i.indexOf("click") > -1 && i.indexOf("touchend") > -1, pe.call(this, {
                        touchstart: ce.muteMouse,
                        touchend: ce.unMuteMouse
                    }, o, {
                        bustClick: r
                    })), typeof n === Me && (e = s.data("handler"), n = e[n], t[t.length - 1] = function(t) {
                        n.call(e, t)
                    }), t[0] = i, pe.apply(s, t), s)
                },
                kendoDestroy: function(e) {
                    return e = e || this.data("kendoNS"), e && this.off("." + e), this
                }
            }), ve.jQuery = I, ve.eventMap = de, ve.timezone = function() {
                function e(e, t) {
                    var n, i, o, r = t[3],
                        s = t[4],
                        a = t[5],
                        l = t[8];
                    return l || (t[8] = l = {}), l[e] ? l[e] : (isNaN(s) ? 0 === s.indexOf("last") ? (n = new Date(Date.UTC(e, d[r] + 1, 1, a[0] - 24, a[1], a[2], 0)), i = u[s.substr(4, 3)], o = n.getUTCDay(), n.setUTCDate(n.getUTCDate() + i - o - (i > o ? 7 : 0))) : s.indexOf(">=") >= 0 && (n = new Date(Date.UTC(e, d[r], s.substr(5), a[0], a[1], a[2], 0)), i = u[s.substr(0, 3)], o = n.getUTCDay(), n.setUTCDate(n.getUTCDate() + i - o + (i < o ? 7 : 0))) : n = new Date(Date.UTC(e, d[r], s, a[0], a[1], a[2], 0)), l[e] = n)
                }

                function t(t, n, i) {
                    var o, r, s, a;
                    return (n = n[i]) ? (s = new Date(t).getUTCFullYear(), n = jQuery.grep(n, function(e) {
                        var t = e[0],
                            n = e[1];
                        return t <= s && (n >= s || t == s && "only" == n || "max" == n)
                    }), n.push(t), n.sort(function(t, n) {
                        return "number" != typeof t && (t = +e(s, t)), "number" != typeof n && (n = +e(s, n)), t - n
                    }), a = n[jQuery.inArray(t, n) - 1] || n[n.length - 1], isNaN(a) ? a : null) : (o = i.split(":"), r = 0, o.length > 1 && (r = 60 * o[0] + +o[1]), [-1e6, "max", "-", "Jan", 1, [0, 0, 0], r, "-"])
                }

                function n(e, t, n) {
                    var i, o, r, s = t[n];
                    if ("string" == typeof s && (s = t[s]), !s) throw Error('Timezone "' + n + '" is either incorrect, or kendo.timezones.min.js is not included.');
                    for (i = s.length - 1; i >= 0 && (o = s[i][3], !(o && e > o)); i--);
                    if (r = s[i + 1], !r) throw Error('Timezone "' + n + '" not found on ' + e + ".");
                    return r
                }

                function i(e, i, o, r) {
                    typeof e != Re && (e = Date.UTC(e.getFullYear(), e.getMonth(), e.getDate(), e.getHours(), e.getMinutes(), e.getSeconds(), e.getMilliseconds()));
                    var s = n(e, i, r);
                    return {
                        zone: s,
                        rule: t(e, o, s[1])
                    }
                }

                function o(e, t) {
                    var n, o, r;
                    return "Etc/UTC" == t || "Etc/GMT" == t ? 0 : (n = i(e, this.zones, this.rules, t), o = n.zone, r = n.rule, ve.parseFloat(r ? o[0] - r[6] : o[0]))
                }

                function r(e, t) {
                    var n = i(e, this.zones, this.rules, t),
                        o = n.zone,
                        r = n.rule,
                        s = o[2];
                    return s.indexOf("/") >= 0 ? s.split("/")[r && +r[6] ? 1 : 0] : s.indexOf("%s") >= 0 ? s.replace("%s", r && "-" != r[7] ? r[7] : "") : s
                }

                function s(e, t, n) {
                    var i, o, r, s = n;
                    return typeof t == Me && (t = this.offset(e, t)), typeof n == Me && (n = this.offset(e, n)), o = e.getTimezoneOffset(), e = new Date(e.getTime() + 6e4 * (t - n)), r = e.getTimezoneOffset(), typeof s == Me && (s = this.offset(e, s)), i = r - o + (n - s), new Date(e.getTime() + 6e4 * i)
                }

                function a(e, t) {
                    return this.convert(e, e.getTimezoneOffset(), t)
                }

                function l(e, t) {
                    return this.convert(e, t, e.getTimezoneOffset())
                }

                function c(e) {
                    return this.apply(new Date(e), "Etc/UTC")
                }
                var d = {
                        Jan: 0,
                        Feb: 1,
                        Mar: 2,
                        Apr: 3,
                        May: 4,
                        Jun: 5,
                        Jul: 6,
                        Aug: 7,
                        Sep: 8,
                        Oct: 9,
                        Nov: 10,
                        Dec: 11
                    },
                    u = {
                        Sun: 0,
                        Mon: 1,
                        Tue: 2,
                        Wed: 3,
                        Thu: 4,
                        Fri: 5,
                        Sat: 6
                    };
                return {
                    zones: {},
                    rules: {},
                    offset: o,
                    convert: s,
                    apply: a,
                    remove: l,
                    abbr: r,
                    toLocalDate: c
                }
            }(), ve.date = function() {
                function e(e, t) {
                    return 0 === t && 23 === e.getHours() && (e.setHours(e.getHours() + 2), !0)
                }

                function t(t, n, i) {
                    var o = t.getHours();
                    i = i || 1, n = (n - t.getDay() + 7 * i) % 7, t.setDate(t.getDate() + n), e(t, o)
                }

                function i(e, n, i) {
                    return e = new Date(e), t(e, n, i), e
                }

                function o(e) {
                    return new Date(e.getFullYear(), e.getMonth(), 1)
                }

                function r(e) {
                    var t = new Date(e.getFullYear(), e.getMonth() + 1, 0),
                        n = o(e),
                        i = Math.abs(t.getTimezoneOffset() - n.getTimezoneOffset());
                    return i && t.setHours(n.getHours() + i / 60), t
                }

                function s(e, t) {
                    return 1 !== t ? f(i(e, t, -1), 4) : f(e, 4 - (e.getDay() || 7))
                }

                function a(e, t) {
                    var n = new Date(e.getFullYear(), 0, 1, (-6)),
                        i = s(e, t),
                        o = i.getTime() - n.getTime(),
                        r = Math.floor(o / y);
                    return 1 + Math.floor(r / 7)
                }

                function l(e, t) {
                    var i, o, r;
                    return t === n && (t = ve.culture().calendar.firstDay), i = f(e, -7), o = f(e, 7), r = a(e, t), 0 === r ? a(i, t) + 1 : 53 === r && a(o, t) > 1 ? 1 : r
                }

                function c(t) {
                    return t = new Date(t.getFullYear(), t.getMonth(), t.getDate(), 0, 0, 0), e(t, 0), t
                }

                function d(e) {
                    return Date.UTC(e.getFullYear(), e.getMonth(), e.getDate(), e.getHours(), e.getMinutes(), e.getSeconds(), e.getMilliseconds())
                }

                function u(e) {
                    return b(e).getTime() - c(b(e))
                }

                function h(e, t, n) {
                    var i, o = u(t),
                        r = u(n);
                    return !e || o == r || (t >= n && (n += y), i = u(e), o > i && (i += y), r < o && (r += y), i >= o && i <= r)
                }

                function p(e, t, n) {
                    var i, o = t.getTime(),
                        r = n.getTime();
                    return o >= r && (r += y), i = e.getTime(), i >= o && i <= r
                }

                function f(t, n) {
                    var i = t.getHours();
                    return t = new Date(t), m(t, n * y), e(t, i), t
                }

                function m(e, t, n) {
                    var i, o = e.getTimezoneOffset();
                    e.setTime(e.getTime() + t), n || (i = e.getTimezoneOffset() - o, e.setTime(e.getTime() + i * w))
                }

                function g(t, n) {
                    return t = new Date(ve.date.getDate(t).getTime() + ve.date.getMilliseconds(n)), e(t, n.getHours()), t
                }

                function v() {
                    return c(new Date)
                }

                function _(e) {
                    return c(e).getTime() == v().getTime()
                }

                function b(e) {
                    var t = new Date(1980, 1, 1, 0, 0, 0);
                    return e && t.setHours(e.getHours(), e.getMinutes(), e.getSeconds(), e.getMilliseconds()), t
                }
                var w = 6e4,
                    y = 864e5;
                return {
                    adjustDST: e,
                    dayOfWeek: i,
                    setDayOfWeek: t,
                    getDate: c,
                    isInDateRange: p,
                    isInTimeRange: h,
                    isToday: _,
                    nextDay: function(e) {
                        return f(e, 1)
                    },
                    previousDay: function(e) {
                        return f(e, -1)
                    },
                    toUtcTime: d,
                    MS_PER_DAY: y,
                    MS_PER_HOUR: 60 * w,
                    MS_PER_MINUTE: w,
                    setTime: m,
                    setHours: g,
                    addDays: f,
                    today: v,
                    toInvariantTime: b,
                    firstDayOfMonth: o,
                    lastDayOfMonth: r,
                    weekInYear: l,
                    getMilliseconds: u
                }
            }(), ve.stripWhitespace = function(e) {
                var t, n, i;
                if (document.createNodeIterator)
                    for (t = document.createNodeIterator(e, NodeFilter.SHOW_TEXT, function(t) {
                            return t.parentNode == e ? NodeFilter.FILTER_ACCEPT : NodeFilter.FILTER_REJECT
                        }, !1); t.nextNode();) t.referenceNode && !t.referenceNode.textContent.trim() && t.referenceNode.parentNode.removeChild(t.referenceNode);
                else
                    for (n = 0; n < e.childNodes.length; n++) i = e.childNodes[n], 3 != i.nodeType || /\S/.test(i.nodeValue) || (e.removeChild(i), n--), 1 == i.nodeType && ve.stripWhitespace(i)
            }, me = t.requestAnimationFrame || t.webkitRequestAnimationFrame || t.mozRequestAnimationFrame || t.oRequestAnimationFrame || t.msRequestAnimationFrame || function(e) {
                setTimeout(e, 1e3 / 60)
            }, ve.animationFrame = function(e) {
                me.call(t, e)
            }, ge = [], ve.queueAnimation = function(e) {
                ge[ge.length] = e, 1 === ge.length && ve.runNextAnimation()
            }, ve.runNextAnimation = function() {
                ve.animationFrame(function() {
                    ge[0] && (ge.shift()(), ge[0] && ve.runNextAnimation())
                })
            }, ve.parseQueryStringParams = function(e) {
                for (var t = e.split("?")[1] || "", n = {}, i = t.split(/&|=/), o = i.length, r = 0; r < o; r += 2) "" !== i[r] && (n[decodeURIComponent(i[r])] = decodeURIComponent(i[r + 1]));
                return n
            }, ve.elementUnderCursor = function(e) {
                if (n !== e.x.client) return document.elementFromPoint(e.x.client, e.y.client)
            }, ve.wheelDeltaY = function(e) {
                var t, i = e.originalEvent,
                    o = i.wheelDeltaY;
                return i.wheelDelta ? (o === n || o) && (t = i.wheelDelta) : i.detail && i.axis === i.VERTICAL_AXIS && (t = 10 * -i.detail), t
            }, ve.throttle = function(e, t) {
                var i, o, r = 0;
                return !t || t <= 0 ? e : (o = function() {
                    function o() {
                        e.apply(s, l), r = +new Date
                    }
                    var s = this,
                        a = +new Date - r,
                        l = arguments;
                    return r ? (i && clearTimeout(i), a > t ? o() : i = setTimeout(o, t - a), n) : o()
                }, o.cancel = function() {
                    clearTimeout(i)
                }, o)
            }, ve.caret = function(t, i, o) {
                var r, s, a, l, c, d = i !== n;
                if (o === n && (o = i), t[0] && (t = t[0]), !d || !t.disabled) {
                    try {
                        t.selectionStart !== n ? d ? (t.focus(), s = Se.mobileOS, s.wp || s.android ? setTimeout(function() {
                            t.setSelectionRange(i, o)
                        }, 0) : t.setSelectionRange(i, o)) : i = [t.selectionStart, t.selectionEnd] : document.selection && (e(t).is(":visible") && t.focus(), r = t.createTextRange(), d ? (r.collapse(!0), r.moveStart("character", i), r.moveEnd("character", o - i), r.select()) : (a = r.duplicate(), r.moveToBookmark(document.selection.createRange().getBookmark()), a.setEndPoint("EndToStart", r), l = a.text.length, c = l + r.text.length, i = [l, c]))
                    } catch (u) {
                        i = []
                    }
                    return i
                }
            }, ve.compileMobileDirective = function(e, n) {
                var i = t.angular;
                return e.attr("data-" + ve.ns + "role", e[0].tagName.toLowerCase().replace("kendo-mobile-", "").replace("-", "")), i.element(e).injector().invoke(["$compile", function(t) {
                    t(e)(n), /^\$(digest|apply)$/.test(n.$$phase) || n.$digest()
                }]), ve.widgetInstance(e, ve.mobile.ui)
            }, ve.antiForgeryTokens = function() {
                var t = {},
                    i = e("meta[name=csrf-token],meta[name=_csrf]").attr("content"),
                    o = e("meta[name=csrf-param],meta[name=_csrf_header]").attr("content");
                return e("input[name^='__RequestVerificationToken']").each(function() {
                    t[this.name] = this.value
                }), o !== n && i !== n && (t[o] = i), t
            }, ve.cycleForm = function(e) {
                function t(e) {
                    var t = ve.widgetInstance(e);
                    t && t.focus ? t.focus() : e.focus()
                }
                var n = e.find("input, .k-widget").first(),
                    i = e.find("button, .k-button").last();
                i.on("keydown", function(e) {
                    e.keyCode != ve.keys.TAB || e.shiftKey || (e.preventDefault(), t(n))
                }), n.on("keydown", function(e) {
                    e.keyCode == ve.keys.TAB && e.shiftKey && (e.preventDefault(), t(i))
                })
            }, ve.focusElement = function(n) {
                var i = [],
                    o = n.parentsUntil("body").filter(function(e, t) {
                        var n = ve.getComputedStyles(t, ["overflow"]);
                        return "visible" !== n.overflow
                    }).add(t);
                o.each(function(t, n) {
                    i[t] = e(n).scrollTop()
                });
                try {
                    n[0].setActive()
                } catch (r) {
                    n[0].focus()
                }
                o.each(function(t, n) {
                    e(n).scrollTop(i[t])
                })
            }, ve.matchesMedia = function(e) {
                var n = ve._bootstrapToMedia(e) || e;
                return Se.matchMedia && t.matchMedia(n).matches
            }, ve._bootstrapToMedia = function(e) {
                return {
                    xs: "(max-width: 576px)",
                    sm: "(min-width: 576px)",
                    md: "(min-width: 768px)",
                    lg: "(min-width: 992px)",
                    xl: "(min-width: 1200px)"
                }[e]
            },
            function() {
                function n(t, n, i, o) {
                    var r, s, a = e("<form>").attr({
                            action: i,
                            method: "POST",
                            target: o
                        }),
                        l = ve.antiForgeryTokens();
                    l.fileName = n, r = t.split(";base64,"), l.contentType = r[0].replace("data:", ""), l.base64 = r[1];
                    for (s in l) l.hasOwnProperty(s) && e("<input>").attr({
                        value: l[s],
                        name: s,
                        type: "hidden"
                    }).appendTo(a);
                    a.appendTo("body").submit().remove()
                }

                function i(e, t) {
                    var n, i, o, r, s, a = e;
                    if ("string" == typeof e) {
                        for (n = e.split(";base64,"), i = n[0], o = atob(n[1]), r = new Uint8Array(o.length), s = 0; s < o.length; s++) r[s] = o.charCodeAt(s);
                        a = new Blob([r.buffer], {
                            type: i
                        })
                    }
                    navigator.msSaveBlob(a, t)
                }

                function o(e, n) {
                    t.Blob && e instanceof Blob && (e = URL.createObjectURL(e)), r.download = n, r.href = e;
                    var i = document.createEvent("MouseEvents");
                    i.initMouseEvent("click", !0, !1, t, 0, 0, 0, 0, 0, !1, !1, !1, !1, 0, null), r.dispatchEvent(i), setTimeout(function() {
                        URL.revokeObjectURL(e)
                    })
                }
                var r = document.createElement("a"),
                    s = "download" in r && !ve.support.browser.edge;
                ve.saveAs = function(e) {
                    var t = n;
                    e.forceProxy || (s ? t = o : navigator.msSaveBlob && (t = i)), t(e.dataURI, e.fileName, e.proxyURL, e.proxyTarget)
                }
            }(), ve.proxyModelSetters = function(e) {
                var t = {};
                return Object.keys(e || {}).forEach(function(n) {
                    Object.defineProperty(t, n, {
                        get: function() {
                            return e[n]
                        },
                        set: function(t) {
                            e[n] = t, e.dirty = !0
                        }
                    })
                }), t
            }
    }(jQuery, window), window.kendo
}, "function" == typeof define && define.amd ? define : function(e, t, n) {
    (n || t)()
}),
function(e, define) {
    define("kendo.data.min", ["kendo.core.min", "kendo.data.odata.min", "kendo.data.xml.min"], e)
}(function() {
    return function(e, t) {
        function n(e, t, n, i) {
            return function(o) {
                var r, s = {};
                for (r in o) s[r] = o[r];
                s.field = i ? n + "." + o.field : n, t == Ie && e._notifyChange && e._notifyChange(s), e.trigger(t, s)
            }
        }

        function i(t, n) {
            if (t === n) return !0;
            var o, r = e.type(t),
                s = e.type(n);
            if (r !== s) return !1;
            if ("date" === r) return t.getTime() === n.getTime();
            if ("object" !== r && "array" !== r) return !1;
            for (o in t)
                if (!i(t[o], n[o])) return !1;
            return !0
        }

        function o(e, t) {
            var n, i;
            for (i in e) {
                if (n = e[i], pe(n) && n.field && n.field === t) return n;
                if (n === t) return n
            }
            return null
        }

        function r(e) {
            this.data = e || []
        }

        function s(e, n) {
            if (e) {
                var i = typeof e === Ce ? {
                        field: e,
                        dir: n
                    } : e,
                    o = me(i) ? i : i !== t ? [i] : [];
                return ge(o, function(e) {
                    return !!e.dir
                })
            }
        }

        function a(e) {
            var t, n, i, o, r = e.filters;
            if (r)
                for (t = 0, n = r.length; t < n; t++) i = r[t], o = i.operator, o && typeof o === Ce && (i.operator = J[o.toLowerCase()] || o), a(i)
        }

        function l(e) {
            if (e && !fe(e)) return !me(e) && e.filters || (e = {
                logic: "and",
                filters: me(e) ? e : [e]
            }), a(e), e
        }

        function c(e, t) {
            return !e.logic && !t.logic && (e.field === t.field && e.value === t.value && e.operator === t.operator)
        }

        function d(e) {
            return e = e || {}, fe(e) ? {
                logic: "and",
                filters: []
            } : l(e)
        }

        function u(e, t) {
            return t.logic || e.field > t.field ? 1 : e.field < t.field ? -1 : 0
        }

        function h(e, t) {
            var n, i, o, r, s;
            if (e = d(e), t = d(t), e.logic !== t.logic) return !1;
            if (o = (e.filters || []).slice(), r = (t.filters || []).slice(), o.length !== r.length) return !1;
            for (o = o.sort(u), r = r.sort(u), s = 0; s < o.length; s++)
                if (n = o[s], i = r[s], n.logic && i.logic) {
                    if (!h(n, i)) return !1
                } else if (!c(n, i)) return !1;
            return !0
        }

        function p(e) {
            return me(e) ? e : [e]
        }

        function f(e, n) {
            var i = typeof e === Ce ? {
                    field: e,
                    dir: n
                } : e,
                o = me(i) ? i : i !== t ? [i] : [];
            return q(o, function(e) {
                return {
                    field: e.field,
                    dir: e.dir || "asc",
                    aggregates: e.aggregates
                }
            })
        }

        function m(e, t) {
            return e && e.getTime && t && t.getTime ? e.getTime() === t.getTime() : e === t
        }

        function g(e, t, n, i, o, r) {
            var s, a, l, c, d;
            for (t = t || [], c = t.length, s = 0; s < c; s++) a = t[s], l = a.aggregate, d = a.field, e[d] = e[d] || {}, r[d] = r[d] || {}, r[d][l] = r[d][l] || {}, e[d][l] = ee[l.toLowerCase()](e[d][l], n, we.accessor(d), i, o, r[d][l])
        }

        function v(e) {
            return "number" == typeof e && !isNaN(e)
        }

        function _(e) {
            return e && e.getTime
        }

        function b(e) {
            var t, n = e.length,
                i = Array(n);
            for (t = 0; t < n; t++) i[t] = e[t].toJSON();
            return i
        }

        function w(e, t, n, i, o) {
            var r, s, a, l, c, d = {};
            for (l = 0, c = e.length; l < c; l++) {
                r = e[l];
                for (s in t) a = o[s], a && a !== s && (d[a] || (d[a] = we.setter(a)), d[a](r, t[s](r)), delete r[s])
            }
        }

        function y(e, t, n, i, o) {
            var r, s, a, l, c;
            for (l = 0, c = e.length; l < c; l++) {
                r = e[l];
                for (s in t) r[s] = n._parse(s, t[s](r)), a = o[s], a && a !== s && delete r[a]
            }
        }

        function k(e, t, n, i, o) {
            var r, s, a, l;
            for (s = 0, l = e.length; s < l; s++) r = e[s], a = i[r.field], a && a != r.field && (r.field = a), r.value = n._parse(r.field, r.value), r.hasSubgroups ? k(r.items, t, n, i, o) : y(r.items, t, n, i, o)
        }

        function x(e, t, n, i, o, r) {
            return function(s) {
                return s = e(s), C(t, n, i, o, r)(s)
            }
        }

        function C(e, t, n, i, o) {
            return function(r) {
                return r && !fe(n) && ("[object Array]" === Ye.call(r) || r instanceof Ze || (r = [r]), t(r, n, new e, i, o)), r || []
            }
        }

        function S(e, t) {
            var n, i, o;
            if (t.items && t.items.length)
                for (o = 0; o < t.items.length; o++) n = e.items[o], i = t.items[o], n && i ? n.hasSubgroups ? S(n, i) : n.field && n.value == i.value ? n.items.push.apply(n.items, i.items) : e.items.push.apply(e.items, [i]) : i && e.items.push.apply(e.items, [i])
        }

        function T(e, t, n, i) {
            for (var o, r, s, a = 0; t.length && i && (o = t[a], r = o.items, s = r.length, e && e.field === o.field && e.value === o.value ? (e.hasSubgroups && e.items.length ? T(e.items[e.items.length - 1], o.items, n, i) : (r = r.slice(n, n + i), e.items = e.items.concat(r)), t.splice(a--, 1)) : o.hasSubgroups && r.length ? (T(o, r, n, i), o.items.length || t.splice(a--, 1)) : (r = r.slice(n, n + i), o.items = r, o.items.length || t.splice(a--, 1)), 0 === r.length ? n -= s : (n = 0, i -= r.length), !(++a >= t.length)););
            a < t.length && t.splice(a, t.length - a)
        }

        function D(e) {
            var t, n, i, o, r, s = [];
            for (t = 0, n = e.length; t < n; t++)
                if (r = e.at(t), r.hasSubgroups) s = s.concat(D(r.items));
                else
                    for (i = r.items, o = 0; o < i.length; o++) s.push(i.at(o));
            return s
        }

        function A(e, t) {
            var n, i, o;
            if (t)
                for (n = 0, i = e.length; n < i; n++) o = e.at(n), o.hasSubgroups ? A(o.items, t) : o.items = new j(o.items, t)
        }

        function E(e, t) {
            for (var n = 0, i = e.length; n < i; n++)
                if (e[n].hasSubgroups) {
                    if (E(e[n].items, t)) return !0
                } else if (t(e[n].items, e[n])) return !0
        }

        function I(e, t, n, i) {
            for (var o = 0; o < e.length && e[o].data !== t && !M(e[o].data, n, i); o++);
        }

        function M(e, t, n) {
            for (var i = 0, o = e.length; i < o; i++) {
                if (e[i] && e[i].hasSubgroups) return M(e[i].items, t, n);
                if (e[i] === t || e[i] === n) return e[i] = n, !0
            }
        }

        function R(e, n, i, o, r) {
            var s, a, l, c;
            for (s = 0, a = e.length; s < a; s++)
                if (l = e[s], l && !(l instanceof o))
                    if (l.hasSubgroups === t || r) {
                        for (c = 0; c < n.length; c++)
                            if (n[c] === l) {
                                e[s] = n.at(c), I(i, n, l, e[s]);
                                break
                            }
                    } else R(l.items, n, i, o, r)
        }

        function F(e, t) {
            var n, i, o = e.length;
            for (i = 0; i < o; i++)
                if (n = e[i], n.uid && n.uid == t.uid) return e.splice(i, 1), n
        }

        function P(e, t) {
            return t ? B(e, function(e) {
                return e.uid && e.uid == t.uid || e[t.idField] === t.id && t.id !== t._defaultId
            }) : -1
        }

        function z(e, t) {
            return t ? B(e, function(e) {
                return e.uid == t.uid
            }) : -1
        }

        function B(e, t) {
            var n, i;
            for (n = 0, i = e.length; n < i; n++)
                if (t(e[n])) return n;
            return -1
        }

        function L(e, t) {
            var n, i;
            return e && !fe(e) ? (n = e[t], i = pe(n) ? n.from || n.field || t : e[t] || t, ye(i) ? t : i) : t
        }

        function H(e, t) {
            var n, i, o, r = {};
            for (o in e) "filters" !== o && (r[o] = e[o]);
            if (e.filters)
                for (r.filters = [], n = 0, i = e.filters.length; n < i; n++) r.filters[n] = H(e.filters[n], t);
            else r.field = L(t.fields, r.field);
            return r
        }

        function N(e, t) {
            var n, i, o, r, s, a = [];
            for (n = 0, i = e.length; n < i; n++) {
                o = {}, r = e[n];
                for (s in r) o[s] = r[s];
                o.field = L(t.fields, o.field), o.aggregates && me(o.aggregates) && (o.aggregates = N(o.aggregates, t)), a.push(o)
            }
            return a
        }

        function O(t, n) {
            var i, o, r, s, a, l, c, d, u, h;
            for (t = e(t)[0], i = t.options, o = n[0], r = n[1], s = [], a = 0, l = i.length; a < l; a++) u = {}, d = i[a], c = d.parentNode, c === t && (c = null), d.disabled || c && c.disabled || (c && (u.optgroup = c.label), u[o.field] = d.text, h = d.attributes.value, h = h && h.specified ? d.value : d.text, u[r.field] = h, s.push(u));
            return s
        }

        function V(t, n) {
            var i, o, r, s, a, l, c, d = e(t)[0].tBodies[0],
                u = d ? d.rows : [],
                h = n.length,
                p = [];
            for (i = 0, o = u.length; i < o; i++) {
                for (a = {}, c = !0, s = u[i].cells, r = 0; r < h; r++) l = s[r], "th" !== l.nodeName.toLowerCase() && (c = !1, a[n[r].field] = l.innerHTML);
                c || p.push(a)
            }
            return p
        }

        function W(e) {
            return function() {
                var t = this._data,
                    n = re.fn[e].apply(this, $e.call(arguments));
                return this._data != t && this._attachBubbleHandlers(), n
            }
        }

        function U(t, n) {
            function i(e, t) {
                return e.filter(t).add(e.find(t))
            }
            var o, r, s, a, l, c, d, u, h = e(t).children(),
                p = [],
                f = n[0].field,
                m = n[1] && n[1].field,
                g = n[2] && n[2].field,
                v = n[3] && n[3].field;
            for (o = 0, r = h.length; o < r; o++) s = {
                _loaded: !0
            }, a = h.eq(o), c = a[0].firstChild, u = a.children(), t = u.filter("ul"), u = u.filter(":not(ul)"), l = a.attr("data-id"), l && (s.id = l), c && (s[f] = 3 == c.nodeType ? c.nodeValue : u.text()), m && (s[m] = i(u, "a").attr("href")), v && (s[v] = i(u, "img").attr("src")), g && (d = i(u, ".k-sprite").prop("className"), s[g] = d && e.trim(d.replace("k-sprite", ""))), t.length && (s.items = U(t.eq(0), n)), "true" == a.attr("data-hasChildren") && (s.hasChildren = !0), p.push(s);
            return p
        }
        var q, j, G, $, K, Y, Q, X, Z, J, ee, te, ne, ie, oe, re, se, ae, le, ce, de, ue = e.extend,
            he = e.proxy,
            pe = e.isPlainObject,
            fe = e.isEmptyObject,
            me = e.isArray,
            ge = e.grep,
            ve = e.ajax,
            _e = e.each,
            be = e.noop,
            we = window.kendo,
            ye = we.isFunction,
            ke = we.Observable,
            xe = we.Class,
            Ce = "string",
            Se = "function",
            Te = "create",
            De = "read",
            Ae = "update",
            Ee = "destroy",
            Ie = "change",
            Me = "sync",
            Re = "get",
            Fe = "error",
            Pe = "requestStart",
            ze = "progress",
            Be = "requestEnd",
            Le = [Te, De, Ae, Ee],
            He = function(e) {
                return e
            },
            Ne = we.getter,
            Oe = we.stringify,
            Ve = Math,
            We = [].push,
            Ue = [].join,
            qe = [].pop,
            je = [].splice,
            Ge = [].shift,
            $e = [].slice,
            Ke = [].unshift,
            Ye = {}.toString,
            Qe = we.support.stableSort,
            Xe = /^\/Date\((.*?)\)\/$/,
            Ze = ke.extend({
                init: function(e, t) {
                    var n = this;
                    n.type = t || G, ke.fn.init.call(n), n.length = e.length, n.wrapAll(e, n)
                },
                at: function(e) {
                    return this[e]
                },
                toJSON: function() {
                    var e, t, n = this.length,
                        i = Array(n);
                    for (e = 0; e < n; e++) t = this[e], t instanceof G && (t = t.toJSON()), i[e] = t;
                    return i
                },
                parent: be,
                wrapAll: function(e, t) {
                    var n, i, o = this,
                        r = function() {
                            return o
                        };
                    for (t = t || [], n = 0, i = e.length; n < i; n++) t[n] = o.wrap(e[n], r);
                    return t
                },
                wrap: function(e, t) {
                    var n, i = this;
                    return null !== e && "[object Object]" === Ye.call(e) && (n = e instanceof i.type || e instanceof Y, n || (e = e instanceof G ? e.toJSON() : e, e = new i.type(e)), e.parent = t, e.bind(Ie, function(e) {
                        i.trigger(Ie, {
                            field: e.field,
                            node: e.node,
                            index: e.index,
                            items: e.items || [this],
                            action: e.node ? e.action || "itemloaded" : "itemchange"
                        })
                    })), e
                },
                push: function() {
                    var e, t = this.length,
                        n = this.wrapAll(arguments);
                    return e = We.apply(this, n), this.trigger(Ie, {
                        action: "add",
                        index: t,
                        items: n
                    }), e
                },
                slice: $e,
                sort: [].sort,
                join: Ue,
                pop: function() {
                    var e = this.length,
                        t = qe.apply(this);
                    return e && this.trigger(Ie, {
                        action: "remove",
                        index: e - 1,
                        items: [t]
                    }), t
                },
                splice: function(e, t, n) {
                    var i, o, r, s = this.wrapAll($e.call(arguments, 2));
                    if (i = je.apply(this, [e, t].concat(s)), i.length)
                        for (this.trigger(Ie, {
                                action: "remove",
                                index: e,
                                items: i
                            }), o = 0, r = i.length; o < r; o++) i[o] && i[o].children && i[o].unbind(Ie);
                    return n && this.trigger(Ie, {
                        action: "add",
                        index: e,
                        items: s
                    }), i
                },
                shift: function() {
                    var e = this.length,
                        t = Ge.apply(this);
                    return e && this.trigger(Ie, {
                        action: "remove",
                        index: 0,
                        items: [t]
                    }), t
                },
                unshift: function() {
                    var e, t = this.wrapAll(arguments);
                    return e = Ke.apply(this, t), this.trigger(Ie, {
                        action: "add",
                        index: 0,
                        items: t
                    }), e
                },
                indexOf: function(e) {
                    var t, n, i = this;
                    for (t = 0, n = i.length; t < n; t++)
                        if (i[t] === e) return t;
                    return -1
                },
                forEach: function(e, t) {
                    for (var n = 0, i = this.length, o = t || window; n < i; n++) e.call(o, this[n], n, this)
                },
                map: function(e, t) {
                    for (var n = 0, i = [], o = this.length, r = t || window; n < o; n++) i[n] = e.call(r, this[n], n, this);
                    return i
                },
                reduce: function(e) {
                    var t, n = 0,
                        i = this.length;
                    for (2 == arguments.length ? t = arguments[1] : n < i && (t = this[n++]); n < i; n++) t = e(t, this[n], n, this);
                    return t
                },
                reduceRight: function(e) {
                    var t, n = this.length - 1;
                    for (2 == arguments.length ? t = arguments[1] : n > 0 && (t = this[n--]); n >= 0; n--) t = e(t, this[n], n, this);
                    return t
                },
                filter: function(e, t) {
                    for (var n, i = 0, o = [], r = this.length, s = t || window; i < r; i++) n = this[i], e.call(s, n, i, this) && (o[o.length] = n);
                    return o
                },
                find: function(e, t) {
                    for (var n, i = 0, o = this.length, r = t || window; i < o; i++)
                        if (n = this[i], e.call(r, n, i, this)) return n
                },
                every: function(e, t) {
                    for (var n, i = 0, o = this.length, r = t || window; i < o; i++)
                        if (n = this[i], !e.call(r, n, i, this)) return !1;
                    return !0
                },
                some: function(e, t) {
                    for (var n, i = 0, o = this.length, r = t || window; i < o; i++)
                        if (n = this[i], e.call(r, n, i, this)) return !0;
                    return !1
                },
                remove: function(e) {
                    var t = this.indexOf(e);
                    t !== -1 && this.splice(t, 1)
                },
                empty: function() {
                    this.splice(0, this.length)
                }
            });
        "undefined" != typeof Symbol && Symbol.iterator && !Ze.prototype[Symbol.iterator] && (Ze.prototype[Symbol.iterator] = [][Symbol.iterator]), j = Ze.extend({
            init: function(e, t) {
                ke.fn.init.call(this), this.type = t || G;
                for (var n = 0; n < e.length; n++) this[n] = e[n];
                this.length = n, this._parent = he(function() {
                    return this
                }, this)
            },
            at: function(e) {
                var t = this[e];
                return t instanceof this.type ? t.parent = this._parent : t = this[e] = this.wrap(t, this._parent), t
            }
        }), G = ke.extend({
            init: function(e) {
                var t, n, i = this,
                    o = function() {
                        return i
                    };
                ke.fn.init.call(this), this._handlers = {};
                for (n in e) t = e[n], "object" == typeof t && t && !t.getTime && "_" != n.charAt(0) && (t = i.wrap(t, n, o)), i[n] = t;
                i.uid = we.guid()
            },
            shouldSerialize: function(e) {
                return this.hasOwnProperty(e) && "_handlers" !== e && "_events" !== e && typeof this[e] !== Se && "uid" !== e
            },
            forEach: function(e) {
                for (var t in this) this.shouldSerialize(t) && e(this[t], t)
            },
            toJSON: function() {
                var e, t, n = {};
                for (t in this) this.shouldSerialize(t) && (e = this[t], (e instanceof G || e instanceof Ze) && (e = e.toJSON()), n[t] = e);
                return n
            },
            get: function(e) {
                var t, n = this;
                return n.trigger(Re, {
                    field: e
                }), t = "this" === e ? n : we.getter(e, !0)(n)
            },
            _set: function(e, t) {
                var n, i, o, r = this,
                    s = e.indexOf(".") >= 0;
                if (s)
                    for (n = e.split("."), i = ""; n.length > 1;) {
                        if (i += n.shift(), o = we.getter(i, !0)(r), o instanceof G) return o.set(n.join("."), t), s;
                        i += "."
                    }
                return we.setter(e)(r, t), s
            },
            set: function(e, t) {
                var n = this,
                    i = !1,
                    o = e.indexOf(".") >= 0,
                    r = we.getter(e, !0)(n);
                return r !== t && (r instanceof ke && this._handlers[e] && (this._handlers[e].get && r.unbind(Re, this._handlers[e].get), r.unbind(Ie, this._handlers[e].change)), i = n.trigger("set", {
                    field: e,
                    value: t
                }), i || (o || (t = n.wrap(t, e, function() {
                    return n
                })), (!n._set(e, t) || e.indexOf("(") >= 0 || e.indexOf("[") >= 0) && n.trigger(Ie, {
                    field: e
                }))), i
            },
            parent: be,
            wrap: function(e, t, i) {
                var o, r, s, a, l = this,
                    c = Ye.call(e);
                return null == e || "[object Object]" !== c && "[object Array]" !== c || (s = e instanceof Ze, a = e instanceof re, "[object Object]" !== c || a || s ? ("[object Array]" === c || s || a) && (s || a || (e = new Ze(e)), r = n(l, Ie, t, !1), e.bind(Ie, r), l._handlers[t] = {
                    change: r
                }) : (e instanceof G || (e = new G(e)), o = n(l, Re, t, !0), e.bind(Re, o), r = n(l, Ie, t, !0), e.bind(Ie, r), l._handlers[t] = {
                    get: o,
                    change: r
                }), e.parent = i), e
            }
        }), $ = {
            number: function(e) {
                return typeof e === Ce && "null" === e.toLowerCase() ? null : we.parseFloat(e)
            },
            date: function(e) {
                return typeof e === Ce && "null" === e.toLowerCase() ? null : we.parseDate(e)
            },
            "boolean": function(e) {
                return typeof e === Ce ? "null" === e.toLowerCase() ? null : "true" === e.toLowerCase() : null != e ? !!e : e
            },
            string: function(e) {
                return typeof e === Ce && "null" === e.toLowerCase() ? null : null != e ? e + "" : e
            },
            "default": function(e) {
                return e
            }
        }, K = {
            string: "",
            number: 0,
            date: new Date,
            "boolean": !1,
            "default": ""
        }, Y = G.extend({
            init: function(n) {
                var i, o, r = this;
                if ((!n || e.isEmptyObject(n)) && (n = e.extend({}, r.defaults, n), r._initializers))
                    for (i = 0; i < r._initializers.length; i++) o = r._initializers[i], n[o] = r.defaults[o]();
                G.fn.init.call(r, n), r.dirty = !1, r.dirtyFields = {}, r.idField && (r.id = r.get(r.idField), r.id === t && (r.id = r._defaultId))
            },
            shouldSerialize: function(e) {
                return G.fn.shouldSerialize.call(this, e) && "uid" !== e && !("id" !== this.idField && "id" === e) && "dirty" !== e && "dirtyFields" !== e && "_accessors" !== e
            },
            _parse: function(e, t) {
                var n, i = this,
                    r = e,
                    s = i.fields || {};
                return e = s[e], e || (e = o(s, r)), e && (n = e.parse, !n && e.type && (n = $[e.type.toLowerCase()])), n ? n(t) : t
            },
            _notifyChange: function(e) {
                var t = e.action;
                "add" != t && "remove" != t || (this.dirty = !0, this.dirtyFields[e.field] = !0)
            },
            editable: function(e) {
                return e = (this.fields || {})[e], !e || e.editable !== !1
            },
            set: function(e, t, n) {
                var o = this,
                    r = o.dirty;
                o.editable(e) && (t = o._parse(e, t), i(t, o.get(e)) ? o.trigger("equalSet", {
                    field: e,
                    value: t
                }) : (o.dirty = !0, o.dirtyFields[e] = !0, G.fn.set.call(o, e, t, n) && !r && (o.dirty = r, o.dirty || (o.dirtyFields[e] = !1))))
            },
            accept: function(e) {
                var t, n, i = this,
                    o = function() {
                        return i
                    };
                for (t in e) n = e[t], "_" != t.charAt(0) && (n = i.wrap(e[t], t, o)), i._set(t, n);
                i.idField && (i.id = i.get(i.idField)), i.dirty = !1, i.dirtyFields = {}
            },
            isNew: function() {
                return this.id === this._defaultId
            }
        }), Y.define = function(e, n) {
            n === t && (n = e, e = Y);
            var i, o, r, s, a, l, c, d, u = ue({
                    defaults: {}
                }, n),
                h = {},
                p = u.id,
                f = [];
            if (p && (u.idField = p), u.id && delete u.id, p && (u.defaults[p] = u._defaultId = ""), "[object Array]" === Ye.call(u.fields)) {
                for (l = 0, c = u.fields.length; l < c; l++) r = u.fields[l], typeof r === Ce ? h[r] = {} : r.field && (h[r.field] = r);
                u.fields = h
            }
            for (o in u.fields) r = u.fields[o], s = r.type || "default", a = null, d = o, o = typeof r.field === Ce ? r.field : o, r.nullable || (a = u.defaults[d !== o ? d : o] = r.defaultValue !== t ? r.defaultValue : K[s.toLowerCase()], "function" == typeof a && f.push(o)), n.id === o && (u._defaultId = a), u.defaults[d !== o ? d : o] = a, r.parse = r.parse || $[s];
            return f.length > 0 && (u._initializers = f), i = e.extend(u), i.define = function(e) {
                return Y.define(i, e)
            }, u.fields && (i.fields = u.fields, i.idField = u.idField), i
        }, Q = {
            selector: function(e) {
                return ye(e) ? e : Ne(e)
            },
            compare: function(e) {
                var t = this.selector(e);
                return function(e, n) {
                    return e = t(e), n = t(n), null == e && null == n ? 0 : null == e ? -1 : null == n ? 1 : e.localeCompare ? e.localeCompare(n) : e > n ? 1 : e < n ? -1 : 0
                }
            },
            create: function(e) {
                var t = e.compare || this.compare(e.field);
                return "desc" == e.dir ? function(e, n) {
                    return t(n, e, !0)
                } : t
            },
            combine: function(e) {
                return function(t, n) {
                    var i, o, r = e[0](t, n);
                    for (i = 1, o = e.length; i < o; i++) r = r || e[i](t, n);
                    return r
                }
            }
        }, X = ue({}, Q, {
            asc: function(e) {
                var t = this.selector(e);
                return function(e, n) {
                    var i = t(e),
                        o = t(n);
                    return i && i.getTime && o && o.getTime && (i = i.getTime(), o = o.getTime()), i === o ? e.__position - n.__position : null == i ? -1 : null == o ? 1 : i.localeCompare ? i.localeCompare(o) : i > o ? 1 : -1
                }
            },
            desc: function(e) {
                var t = this.selector(e);
                return function(e, n) {
                    var i = t(e),
                        o = t(n);
                    return i && i.getTime && o && o.getTime && (i = i.getTime(), o = o.getTime()), i === o ? e.__position - n.__position : null == i ? 1 : null == o ? -1 : o.localeCompare ? o.localeCompare(i) : i < o ? 1 : -1
                }
            },
            create: function(e) {
                return this[e.dir](e.field)
            }
        }), q = function(e, t) {
            var n, i = e.length,
                o = Array(i);
            for (n = 0; n < i; n++) o[n] = t(e[n], n, e);
            return o
        }, Z = function() {
            function e(e) {
                return "string" == typeof e && (e = e.replace(/[\r\n]+/g, "")), JSON.stringify(e)
            }

            function t(t) {
                return function(n, i, o) {
                    return i += "", o && (n = "(" + n + " || '').toString().toLowerCase()", i = i.toLowerCase()), t(n, e(i), o)
                }
            }

            function n(t, n, i, o) {
                if (null != i) {
                    if (typeof i === Ce) {
                        var r = Xe.exec(i);
                        r ? i = new Date((+r[1])) : o ? (i = e(i.toLowerCase()), n = "((" + n + " || '')+'').toLowerCase()") : i = e(i)
                    }
                    i.getTime && (n = "(" + n + "&&" + n + ".getTime?" + n + ".getTime():" + n + ")", i = i.getTime())
                }
                return n + " " + t + " " + i
            }

            function i(e) {
                var t, n, i, o;
                for (t = "/^", n = !1, i = 0; i < e.length; ++i) {
                    if (o = e.charAt(i), n) t += "\\" + o;
                    else {
                        if ("~" == o) {
                            n = !0;
                            continue
                        }
                        t += "*" == o ? ".*" : "?" == o ? "." : ".+^$()[]{}|\\/\n\r\u2028\u2029 ".indexOf(o) >= 0 ? "\\" + o : o
                    }
                    n = !1
                }
                return t + "$/"
            }
            return {
                quote: function(t) {
                    return t && t.getTime ? "new Date(" + t.getTime() + ")" : e(t)
                },
                eq: function(e, t, i) {
                    return n("==", e, t, i)
                },
                neq: function(e, t, i) {
                    return n("!=", e, t, i)
                },
                gt: function(e, t, i) {
                    return n(">", e, t, i)
                },
                gte: function(e, t, i) {
                    return n(">=", e, t, i)
                },
                lt: function(e, t, i) {
                    return n("<", e, t, i)
                },
                lte: function(e, t, i) {
                    return n("<=", e, t, i)
                },
                startswith: t(function(e, t) {
                    return e + ".lastIndexOf(" + t + ", 0) == 0"
                }),
                doesnotstartwith: t(function(e, t) {
                    return e + ".lastIndexOf(" + t + ", 0) == -1"
                }),
                endswith: t(function(e, t) {
                    var n = t ? t.length - 2 : 0;
                    return e + ".indexOf(" + t + ", " + e + ".length - " + n + ") >= 0"
                }),
                doesnotendwith: t(function(e, t) {
                    var n = t ? t.length - 2 : 0;
                    return e + ".indexOf(" + t + ", " + e + ".length - " + n + ") < 0"
                }),
                contains: t(function(e, t) {
                    return e + ".indexOf(" + t + ") >= 0"
                }),
                doesnotcontain: t(function(e, t) {
                    return e + ".indexOf(" + t + ") == -1"
                }),
                matches: t(function(e, t) {
                    return t = t.substring(1, t.length - 1), i(t) + ".test(" + e + ")"
                }),
                doesnotmatch: t(function(e, t) {
                    return t = t.substring(1, t.length - 1), "!" + i(t) + ".test(" + e + ")"
                }),
                isempty: function(e) {
                    return e + " === ''"
                },
                isnotempty: function(e) {
                    return e + " !== ''"
                },
                isnull: function(e) {
                    return "(" + e + " == null)"
                },
                isnotnull: function(e) {
                    return "(" + e + " != null)"
                },
                isnullorempty: function(e) {
                    return "(" + e + " === null) || (" + e + " === '')"
                },
                isnotnullorempty: function(e) {
                    return "(" + e + " !== null) && (" + e + " !== '')"
                }
            }
        }(), r.filterExpr = function(e) {
            var n, i, o, s, a, l, c = [],
                d = {
                    and: " && ",
                    or: " || "
                },
                u = [],
                h = [],
                p = e.filters;
            for (n = 0, i = p.length; n < i; n++) o = p[n], a = o.field, l = o.operator, o.filters ? (s = r.filterExpr(o), o = s.expression.replace(/__o\[(\d+)\]/g, function(e, t) {
                return t = +t, "__o[" + (h.length + t) + "]"
            }).replace(/__f\[(\d+)\]/g, function(e, t) {
                return t = +t, "__f[" + (u.length + t) + "]"
            }), h.push.apply(h, s.operators), u.push.apply(u, s.fields)) : (typeof a === Se ? (s = "__f[" + u.length + "](d)", u.push(a)) : s = we.expr(a), typeof l === Se ? (o = "__o[" + h.length + "](" + s + ", " + Z.quote(o.value) + ")", h.push(l)) : o = Z[(l || "eq").toLowerCase()](s, o.value, o.ignoreCase === t || o.ignoreCase)), c.push(o);
            return {
                expression: "(" + c.join(d[e.logic]) + ")",
                fields: u,
                operators: h
            }
        }, J = {
            "==": "eq",
            equals: "eq",
            isequalto: "eq",
            equalto: "eq",
            equal: "eq",
            "!=": "neq",
            ne: "neq",
            notequals: "neq",
            isnotequalto: "neq",
            notequalto: "neq",
            notequal: "neq",
            "<": "lt",
            islessthan: "lt",
            lessthan: "lt",
            less: "lt",
            "<=": "lte",
            le: "lte",
            islessthanorequalto: "lte",
            lessthanequal: "lte",
            ">": "gt",
            isgreaterthan: "gt",
            greaterthan: "gt",
            greater: "gt",
            ">=": "gte",
            isgreaterthanorequalto: "gte",
            greaterthanequal: "gte",
            ge: "gte",
            notsubstringof: "doesnotcontain",
            isnull: "isnull",
            isempty: "isempty",
            isnotempty: "isnotempty"
        }, r.normalizeFilter = l, r.compareFilters = h, r.prototype = {
            toArray: function() {
                return this.data
            },
            range: function(e, t) {
                return new r(this.data.slice(e, e + t))
            },
            skip: function(e) {
                return new r(this.data.slice(e))
            },
            take: function(e) {
                return new r(this.data.slice(0, e))
            },
            select: function(e) {
                return new r(q(this.data, e))
            },
            order: function(e, t, n) {
                var i = {
                    dir: t
                };
                return e && (e.compare ? i.compare = e.compare : i.field = e), new r(n ? this.data.sort(Q.create(i)) : this.data.slice(0).sort(Q.create(i)))
            },
            orderBy: function(e, t) {
                return this.order(e, "asc", t)
            },
            orderByDescending: function(e, t) {
                return this.order(e, "desc", t)
            },
            sort: function(e, t, n, i) {
                var o, r, a = s(e, t),
                    l = [];
                if (n = n || Q, a.length) {
                    for (o = 0, r = a.length; o < r; o++) l.push(n.create(a[o]));
                    return this.orderBy({
                        compare: n.combine(l)
                    }, i)
                }
                return this
            },
            filter: function(e) {
                var t, n, i, o, s, a, c, d, u = this.data,
                    h = [];
                if (e = l(e), !e || 0 === e.filters.length) return this;
                for (o = r.filterExpr(e), a = o.fields, c = o.operators, s = d = Function("d, __f, __o", "return " + o.expression), (a.length || c.length) && (d = function(e) {
                        return s(e, a, c)
                    }), t = 0, i = u.length; t < i; t++) n = u[t], d(n) && h.push(n);
                return new r(h)
            },
            group: function(e, t) {
                e = f(e || []), t = t || this.data;
                var n, i = this,
                    o = new r(i.data);
                return e.length > 0 && (n = e[0], o = o.groupBy(n).select(function(i) {
                    var o = new r(t).filter([{
                        field: i.field,
                        operator: "eq",
                        value: i.value,
                        ignoreCase: !1
                    }]);
                    return {
                        field: i.field,
                        value: i.value,
                        items: e.length > 1 ? new r(i.items).group(e.slice(1), o.toArray()).toArray() : i.items,
                        hasSubgroups: e.length > 1,
                        aggregates: o.aggregate(n.aggregates)
                    }
                })), o
            },
            groupBy: function(e) {
                if (fe(e) || !this.data.length) return new r([]);
                var t, n, i, o, s = e.field,
                    a = this._sortForGrouping(s, e.dir || "asc"),
                    l = we.accessor(s),
                    c = l.get(a[0], s),
                    d = {
                        field: s,
                        value: c,
                        items: []
                    },
                    u = [d];
                for (i = 0, o = a.length; i < o; i++) t = a[i], n = l.get(t, s), m(c, n) || (c = n, d = {
                    field: s,
                    value: c,
                    items: []
                }, u.push(d)), d.items.push(t);
                return new r(u)
            },
            _sortForGrouping: function(e, t) {
                var n, i, o = this.data;
                if (!Qe) {
                    for (n = 0, i = o.length; n < i; n++) o[n].__position = n;
                    for (o = new r(o).sort(e, t, X).toArray(), n = 0, i = o.length; n < i; n++) delete o[n].__position;
                    return o
                }
                return this.sort(e, t).toArray()
            },
            aggregate: function(e) {
                var t, n, i = {},
                    o = {};
                if (e && e.length)
                    for (t = 0, n = this.data.length; t < n; t++) g(i, e, this.data[t], t, n, o);
                return i
            }
        }, ee = {
            sum: function(e, t, n) {
                var i = n.get(t);
                return v(e) ? v(i) && (e += i) : e = i, e
            },
            count: function(e) {
                return (e || 0) + 1
            },
            average: function(e, n, i, o, r, s) {
                var a = i.get(n);
                return s.count === t && (s.count = 0), v(e) ? v(a) && (e += a) : e = a, v(a) && s.count++, o == r - 1 && v(e) && (e /= s.count), e
            },
            max: function(e, t, n) {
                var i = n.get(t);
                return v(e) || _(e) || (e = i), e < i && (v(i) || _(i)) && (e = i), e
            },
            min: function(e, t, n) {
                var i = n.get(t);
                return v(e) || _(e) || (e = i), e > i && (v(i) || _(i)) && (e = i), e
            }
        }, r.normalizeGroup = f, r.normalizeSort = s, r.process = function(e, n, i) {
            n = n || {};
            var o, a = new r(e),
                l = n.group,
                c = f(l || []).concat(s(n.sort || [])),
                d = n.filterCallback,
                u = n.filter,
                h = n.skip,
                p = n.take;
            return c && i && (a = a.sort(c, t, t, i)), u && (a = a.filter(u), d && (a = d(a)), o = a.toArray().length), c && !i && (a = a.sort(c), l && (e = a.toArray())), h !== t && p !== t && (a = a.range(h, p)), l && (a = a.group(l, e)), {
                total: o,
                data: a.toArray()
            }
        }, te = xe.extend({
            init: function(e) {
                this.data = e.data
            },
            read: function(e) {
                e.success(this.data)
            },
            update: function(e) {
                e.success(e.data)
            },
            create: function(e) {
                e.success(e.data)
            },
            destroy: function(e) {
                e.success(e.data)
            }
        }), ne = xe.extend({
            init: function(e) {
                var t, n = this;
                e = n.options = ue({}, n.options, e), _e(Le, function(t, n) {
                    typeof e[n] === Ce && (e[n] = {
                        url: e[n]
                    })
                }), n.cache = e.cache ? ie.create(e.cache) : {
                    find: be,
                    add: be
                }, t = e.parameterMap, e.submit && (n.submit = e.submit), ye(e.push) && (n.push = e.push), n.push || (n.push = He), n.parameterMap = ye(t) ? t : function(e) {
                    var n = {};
                    return _e(e, function(e, i) {
                        e in t && (e = t[e], pe(e) && (i = e.value(i), e = e.key)), n[e] = i
                    }), n
                }
            },
            options: {
                parameterMap: He
            },
            create: function(e) {
                return ve(this.setup(e, Te))
            },
            read: function(n) {
                var i, o, r, s = this,
                    a = s.cache;
                n = s.setup(n, De), i = n.success || be, o = n.error || be, r = a.find(n.data), r !== t ? i(r) : (n.success = function(e) {
                    a.add(n.data, e), i(e)
                }, e.ajax(n))
            },
            update: function(e) {
                return ve(this.setup(e, Ae))
            },
            destroy: function(e) {
                return ve(this.setup(e, Ee))
            },
            setup: function(e, t) {
                e = e || {};
                var n, i = this,
                    o = i.options[t],
                    r = ye(o.data) ? o.data(e.data) : o.data;
                return e = ue(!0, {}, o, e), n = ue(!0, {}, r, e.data), e.data = i.parameterMap(n, t), ye(e.url) && (e.url = e.url(n)), e
            }
        }), ie = xe.extend({
            init: function() {
                this._store = {}
            },
            add: function(e, n) {
                e !== t && (this._store[Oe(e)] = n)
            },
            find: function(e) {
                return this._store[Oe(e)]
            },
            clear: function() {
                this._store = {}
            },
            remove: function(e) {
                delete this._store[Oe(e)]
            }
        }), ie.create = function(e) {
            var t = {
                inmemory: function() {
                    return new ie
                }
            };
            return pe(e) && ye(e.find) ? e : e === !0 ? new ie : t[e]()
        }, oe = xe.extend({
            init: function(e) {
                var t, n, i, o, r, s, a, l, c, d, u, h, p, f, m = this;
                e = e || {};
                for (t in e) n = e[t], m[t] = typeof n === Ce ? Ne(n) : n;
                o = e.modelBase || Y, pe(m.model) && (m.model = i = o.define(m.model)), r = he(m.data, m), m._dataAccessFunction = r, m.model && (s = he(m.groups, m), a = he(m.serialize, m), l = {}, c = {}, d = {}, u = {}, h = !1, i = m.model, i.fields && (_e(i.fields, function(e, t) {
                    var n;
                    p = e, pe(t) && t.field ? p = t.field : typeof t === Ce && (p = t), pe(t) && t.from && (n = t.from), h = h || n && n !== e || p !== e, f = n || p, c[e] = f.indexOf(".") !== -1 ? Ne(f, !0) : Ne(f), d[e] = Ne(e), l[n || p] = e, u[e] = n || p
                }), !e.serialize && h && (m.serialize = x(a, i, w, d, l, u))), m._dataAccessFunction = r, m._wrapDataAccessBase = C(i, y, c, l, u), m.data = x(r, i, y, c, l, u), m.groups = x(s, i, k, c, l, u))
            },
            errors: function(e) {
                return e ? e.errors : null
            },
            parse: He,
            data: He,
            total: function(e) {
                return e.length
            },
            groups: He,
            aggregates: function() {
                return {}
            },
            serialize: function(e) {
                return e
            }
        }), re = ke.extend({
            init: function(e) {
                var n, i, o, r = this;
                e && (i = e.data), e = r.options = ue({}, r.options, e), r._map = {}, r._prefetch = {}, r._data = [], r._pristineData = [], r._ranges = [], r._view = [], r._pristineTotal = 0, r._destroyed = [], r._pageSize = e.pageSize, r._page = e.page || (e.pageSize ? 1 : t), r._sort = s(e.sort), r._filter = l(e.filter), r._group = f(e.group), r._aggregate = e.aggregate, r._total = e.total, r._shouldDetachObservableParents = !0, ke.fn.init.call(r), r.transport = se.create(e, i, r), ye(r.transport.push) && r.transport.push({
                    pushCreate: he(r._pushCreate, r),
                    pushUpdate: he(r._pushUpdate, r),
                    pushDestroy: he(r._pushDestroy, r)
                }), null != e.offlineStorage && ("string" == typeof e.offlineStorage ? (o = e.offlineStorage, r._storage = {
                    getItem: function() {
                        return JSON.parse(localStorage.getItem(o))
                    },
                    setItem: function(e) {
                        localStorage.setItem(o, Oe(r.reader.serialize(e)))
                    }
                }) : r._storage = e.offlineStorage), r.reader = new we.data.readers[e.schema.type || "json"](e.schema), n = r.reader.model || {}, r._detachObservableParents(), r._data = r._observe(r._data), r._online = !0, r.bind(["push", Fe, Ie, Pe, Me, Be, ze], e)
            },
            options: {
                data: null,
                schema: {
                    modelBase: Y
                },
                offlineStorage: null,
                serverSorting: !1,
                serverPaging: !1,
                serverFiltering: !1,
                serverGrouping: !1,
                serverAggregates: !1,
                batch: !1,
                inPlaceSort: !1
            },
            clone: function() {
                return this
            },
            online: function(n) {
                return n !== t ? this._online != n && (this._online = n, n) ? this.sync() : e.Deferred().resolve().promise() : this._online
            },
            offlineData: function(e) {
                return null == this.options.offlineStorage ? null : e !== t ? this._storage.setItem(e) : this._storage.getItem() || []
            },
            _isServerGrouped: function() {
                var e = this.group() || [];
                return this.options.serverGrouping && e.length
            },
            _pushCreate: function(e) {
                this._push(e, "pushCreate")
            },
            _pushUpdate: function(e) {
                this._push(e, "pushUpdate")
            },
            _pushDestroy: function(e) {
                this._push(e, "pushDestroy")
            },
            _push: function(e, t) {
                var n = this._readData(e);
                n || (n = e), this[t](n)
            },
            _flatData: function(e, t) {
                if (e) {
                    if (this._isServerGrouped()) return D(e);
                    if (!t)
                        for (var n = 0; n < e.length; n++) e.at(n)
                }
                return e
            },
            parent: be,
            get: function(e) {
                var t, n, i = this._flatData(this._data, this.options.useRanges);
                for (t = 0, n = i.length; t < n; t++)
                    if (i[t].id == e) return i[t]
            },
            getByUid: function(e) {
                return this._getByUid(e, this._data)
            },
            _getByUid: function(e, t) {
                var n, i, o = this._flatData(t, this.options.useRanges);
                if (o)
                    for (n = 0, i = o.length; n < i; n++)
                        if (o[n].uid == e) return o[n]
            },
            indexOf: function(e) {
                return z(this._data, e)
            },
            at: function(e) {
                return this._data.at(e)
            },
            data: function(e) {
                var n, i = this;
                if (e === t) {
                    if (i._data)
                        for (n = 0; n < i._data.length; n++) i._data.at(n);
                    return i._data
                }
                i._detachObservableParents(), i._data = this._observe(e), i._pristineData = e.slice(0), i._storeData(), i._ranges = [], i.trigger("reset"), i._addRange(i._data), i._total = i._data.length, i._pristineTotal = i._total, i._process(i._data)
            },
            view: function(e) {
                return e === t ? this._view : (this._view = this._observeView(e), t)
            },
            _observeView: function(e) {
                var t, n = this;
                return R(e, n._data, n._ranges, n.reader.model || G, n._isServerGrouped()), t = new j(e, n.reader.model), t.parent = function() {
                    return n.parent()
                }, t
            },
            flatView: function() {
                var e = this.group() || [];
                return e.length ? D(this._view) : this._view
            },
            add: function(e) {
                return this.insert(this._data.length, e)
            },
            _createNewModel: function(e) {
                return this.reader.model ? new this.reader.model(e) : e instanceof G ? e : new G(e)
            },
            insert: function(e, t) {
                return t || (t = e, e = 0), t instanceof Y || (t = this._createNewModel(t)), this._isServerGrouped() ? this._data.splice(e, 0, this._wrapInEmptyGroup(t)) : this._data.splice(e, 0, t), this._insertModelInRange(e, t), t
            },
            pushInsert: function(t, n) {
                var i, o, r, s, a, l, c = this,
                    d = c._getCurrentRangeSpan();
                n || (n = t, t = 0), me(n) || (n = [n]), i = [], o = this.options.autoSync, this.options.autoSync = !1;
                try {
                    for (r = 0; r < n.length; r++) s = n[r], a = this.insert(t, s), i.push(a), l = a.toJSON(), this._isServerGrouped() && (l = this._wrapInEmptyGroup(l)), this._pristineData.push(l), d && d.length && e(d).last()[0].pristineData.push(l), t++
                } finally {
                    this.options.autoSync = o
                }
                i.length && this.trigger("push", {
                    type: "create",
                    items: i
                })
            },
            pushCreate: function(e) {
                this.pushInsert(this._data.length, e)
            },
            pushUpdate: function(e) {
                var t, n, i, o, r;
                for (me(e) || (e = [e]), t = [], n = 0; n < e.length; n++) i = e[n], o = this._createNewModel(i), r = this.get(o.id), r ? (t.push(r), r.accept(i), r.trigger(Ie), this._updatePristineForModel(r, i)) : this.pushCreate(i);
                t.length && this.trigger("push", {
                    type: "update",
                    items: t
                })
            },
            pushDestroy: function(e) {
                var t = this._removeItems(e);
                t.length && this.trigger("push", {
                    type: "destroy",
                    items: t
                })
            },
            _removeItems: function(e, n) {
                var i, o, r, s, a, l, c;
                me(e) || (e = [e]), i = t === n || n, o = [], r = this.options.autoSync, this.options.autoSync = !1;
                try {
                    for (s = 0; s < e.length; s++) a = e[s], l = this._createNewModel(a), c = !1, this._eachItem(this._data, function(e) {
                        var t, n;
                        for (t = 0; t < e.length; t++)
                            if (n = e.at(t), n.id === l.id) {
                                o.push(n), e.splice(t, 1), c = !0;
                                break
                            }
                    }), c && i && (this._removePristineForModel(l), this._destroyed.pop())
                } finally {
                    this.options.autoSync = r
                }
                return o
            },
            remove: function(e) {
                var t, n = this,
                    i = n._isServerGrouped();
                return this._eachItem(n._data, function(o) {
                    if (t = F(o, e), t && i) return t.isNew && t.isNew() || n._destroyed.push(t), !0
                }), this._removeModelFromRanges(e), e
            },
            destroyed: function() {
                return this._destroyed
            },
            created: function() {
                var e, t, n = [],
                    i = this._flatData(this._data, this.options.useRanges);
                for (e = 0, t = i.length; e < t; e++) i[e].isNew && i[e].isNew() && n.push(i[e]);
                return n
            },
            updated: function() {
                var e, t, n = [],
                    i = this._flatData(this._data, this.options.useRanges);
                for (e = 0, t = i.length; e < t; e++) i[e].isNew && !i[e].isNew() && i[e].dirty && n.push(i[e]);
                return n
            },
            sync: function() {
                var t, n = this,
                    i = [],
                    o = [],
                    r = n._destroyed,
                    s = e.Deferred().resolve().promise();
                if (n.online()) {
                    if (!n.reader.model) return s;
                    i = n.created(), o = n.updated(), t = [], n.options.batch && n.transport.submit ? t = n._sendSubmit(i, o, r) : (t.push.apply(t, n._send("create", i)), t.push.apply(t, n._send("update", o)), t.push.apply(t, n._send("destroy", r))), s = e.when.apply(null, t).then(function() {
                        var e, t;
                        for (e = 0, t = arguments.length; e < t; e++) arguments[e] && n._accept(arguments[e]);
                        n._storeData(!0), n._syncEnd(), n._change({
                            action: "sync"
                        }), n.trigger(Me)
                    })
                } else n._storeData(!0), n._syncEnd(), n._change({
                    action: "sync"
                });
                return s
            },
            _syncEnd: be,
            cancelChanges: function(e) {
                var t = this;
                e instanceof we.data.Model ? t._cancelModel(e) : (t._destroyed = [], t._detachObservableParents(), t._data = t._observe(t._pristineData), t.options.serverPaging && (t._total = t._pristineTotal), t._ranges = [], t._addRange(t._data, 0), t._changesCanceled(), t._change(), t._markOfflineUpdatesAsDirty())
            },
            _changesCanceled: be,
            _markOfflineUpdatesAsDirty: function() {
                var e = this;
                null != e.options.offlineStorage && e._eachItem(e._data, function(e) {
                    var t, n;
                    for (t = 0; t < e.length; t++) n = e.at(t), "update" != n.__state__ && "create" != n.__state__ || (n.dirty = !0)
                })
            },
            hasChanges: function() {
                var e, t, n = this._flatData(this._data, this.options.useRanges);
                if (this._destroyed.length) return !0;
                for (e = 0, t = n.length; e < t; e++)
                    if (n[e].isNew && n[e].isNew() || n[e].dirty) return !0;
                return !1
            },
            _accept: function(t) {
                var n, i = this,
                    o = t.models,
                    r = t.response,
                    s = 0,
                    a = i._isServerGrouped(),
                    l = i._pristineData,
                    c = t.type;
                if (i.trigger(Be, {
                        response: r,
                        type: c
                    }), r && !fe(r)) {
                    if (r = i.reader.parse(r), i._handleCustomErrors(r)) return;
                    r = i.reader.data(r), me(r) || (r = [r])
                } else r = e.map(o, function(e) {
                    return e.toJSON()
                });
                for ("destroy" === c && (i._destroyed = []), s = 0, n = o.length; s < n; s++) "destroy" !== c ? (o[s].accept(r[s]), "create" === c ? l.push(a ? i._wrapInEmptyGroup(o[s]) : r[s]) : "update" === c && i._updatePristineForModel(o[s], r[s])) : i._removePristineForModel(o[s])
            },
            _updatePristineForModel: function(e, t) {
                this._executeOnPristineForModel(e, function(e, n) {
                    we.deepExtend(n[e], t)
                })
            },
            _executeOnPristineForModel: function(e, t) {
                this._eachPristineItem(function(n) {
                    var i = P(n, e);
                    if (i > -1) return t(i, n), !0
                })
            },
            _removePristineForModel: function(e) {
                this._executeOnPristineForModel(e, function(e, t) {
                    t.splice(e, 1)
                })
            },
            _readData: function(e) {
                var t = this._isServerGrouped() ? this.reader.groups : this.reader.data;
                return t.call(this.reader, e)
            },
            _eachPristineItem: function(e) {
                var t = this,
                    n = t.options,
                    i = t._getCurrentRangeSpan();
                t._eachItem(t._pristineData, e), n.serverPaging && n.useRanges && _e(i, function(n, i) {
                    t._eachItem(i.pristineData, e)
                })
            },
            _eachItem: function(e, t) {
                e && e.length && (this._isServerGrouped() ? E(e, t) : t(e))
            },
            _pristineForModel: function(e) {
                var t, n, i = function(i) {
                    if (n = P(i, e), n > -1) return t = i[n], !0
                };
                return this._eachPristineItem(i), t
            },
            _cancelModel: function(e) {
                var t = this,
                    n = this._pristineForModel(e);
                this._eachItem(this._data, function(i) {
                    var o = z(i, e);
                    o >= 0 && (!n || e.isNew() && !n.__state__ ? (t._modelCanceled(e), i.splice(o, 1), t._removeModelFromRanges(e)) : (i[o].accept(n), "update" == n.__state__ && (i[o].dirty = !0)))
                })
            },
            _modelCanceled: be,
            _submit: function(t, n) {
                var i = this;
                i.trigger(Pe, {
                    type: "submit"
                }), i.trigger(ze), i.transport.submit(ue({
                    success: function(n, i) {
                        var o = e.grep(t, function(e) {
                            return e.type == i
                        })[0];
                        o && o.resolve({
                            response: n,
                            models: o.models,
                            type: i
                        })
                    },
                    error: function(e, n, o) {
                        for (var r = 0; r < t.length; r++) t[r].reject(e);
                        i.error(e, n, o)
                    }
                }, n))
            },
            _sendSubmit: function(t, n, i) {
                var o = this,
                    r = [];
                return o.options.batch && (t.length && r.push(e.Deferred(function(e) {
                    e.type = "create", e.models = t
                })), n.length && r.push(e.Deferred(function(e) {
                    e.type = "update", e.models = n
                })), i.length && r.push(e.Deferred(function(e) {
                    e.type = "destroy", e.models = i
                })), o._submit(r, {
                    data: {
                        created: o.reader.serialize(b(t)),
                        updated: o.reader.serialize(b(n)),
                        destroyed: o.reader.serialize(b(i))
                    }
                })), r
            },
            _promise: function(t, n, i) {
                var o = this;
                return e.Deferred(function(e) {
                    o.trigger(Pe, {
                        type: i
                    }), o.trigger(ze), o.transport[i].call(o.transport, ue({
                        success: function(t) {
                            e.resolve({
                                response: t,
                                models: n,
                                type: i
                            })
                        },
                        error: function(t, n, i) {
                            e.reject(t), o.error(t, n, i)
                        }
                    }, t))
                }).promise()
            },
            _send: function(e, t) {
                var n, i, o = this,
                    r = [],
                    s = o.reader.serialize(b(t));
                if (o.options.batch) t.length && r.push(o._promise({
                    data: {
                        models: s
                    }
                }, t, e));
                else
                    for (n = 0, i = t.length; n < i; n++) r.push(o._promise({
                        data: s[n]
                    }, [t[n]], e));
                return r
            },
            read: function(t) {
                var n = this,
                    i = n._params(t),
                    o = e.Deferred();
                return n._queueRequest(i, function() {
                    var e = n.trigger(Pe, {
                        type: "read"
                    });
                    e ? (n._dequeueRequest(), o.resolve(e)) : (n.trigger(ze), n._ranges = [], n.trigger("reset"), n.online() ? n.transport.read({
                        data: i,
                        success: function(e) {
                            n._ranges = [], n.success(e, i), o.resolve()
                        },
                        error: function() {
                            var e = $e.call(arguments);
                            n.error.apply(n, e), o.reject.apply(o, e)
                        }
                    }) : null != n.options.offlineStorage && (n.success(n.offlineData(), i), o.resolve()))
                }), o.promise()
            },
            _readAggregates: function(e) {
                return this.reader.aggregates(e)
            },
            success: function(e) {
                var n, i, o, r, s, a, l, c, d, u = this,
                    h = u.options;
                if (u.trigger(Be, {
                        response: e,
                        type: "read"
                    }), u.online()) {
                    if (e = u.reader.parse(e), u._handleCustomErrors(e)) return u._dequeueRequest(), t;
                    u._total = u.reader.total(e), u._pageSize > u._total && (u._pageSize = u._total, u.options.pageSize && u.options.pageSize > u._pageSize && (u._pageSize = u.options.pageSize)), u._aggregate && h.serverAggregates && (u._aggregateResult = u._readAggregates(e)), e = u._readData(e), u._destroyed = []
                } else {
                    for (e = u._readData(e), n = [], i = {}, o = u.reader.model, r = o ? o.idField : "id", s = 0; s < this._destroyed.length; s++) a = this._destroyed[s][r], i[a] = a;
                    for (s = 0; s < e.length; s++) l = e[s], c = l.__state__, "destroy" == c ? i[l[r]] || this._destroyed.push(this._createNewModel(l)) : n.push(l);
                    e = n, u._total = e.length
                }
                if (u._pristineTotal = u._total, u._pristineData = e.slice(0), u._detachObservableParents(), u.options.endless) {
                    for (u._data.unbind(Ie, u._changeHandler), u._isServerGrouped() && u._data[u._data.length - 1].value === e[0].value && (S(u._data[u._data.length - 1], e[0]), e.shift()), e = u._observe(e), d = 0; d < e.length; d++) u._data.push(e[d]);
                    u._data.bind(Ie, u._changeHandler)
                } else u._data = u._observe(e);
                u._markOfflineUpdatesAsDirty(), u._storeData(), u._addRange(u._data), u._process(u._data), u._dequeueRequest()
            },
            _detachObservableParents: function() {
                if (this._data && this._shouldDetachObservableParents)
                    for (var e = 0; e < this._data.length; e++) this._data[e].parent && (this._data[e].parent = be)
            },
            _storeData: function(e) {
                function t(e) {
                    var n, i, o, r = [];
                    for (n = 0; n < e.length; n++) i = e.at(n), o = i.toJSON(), s && i.items ? o.items = t(i.items) : (o.uid = i.uid, a && (i.isNew() ? o.__state__ = "create" : i.dirty && (o.__state__ = "update"))), r.push(o);
                    return r
                }
                var n, i, o, r, s = this._isServerGrouped(),
                    a = this.reader.model;
                if (null != this.options.offlineStorage) {
                    for (n = t(this._data), i = [], o = 0; o < this._destroyed.length; o++) r = this._destroyed[o].toJSON(), r.__state__ = "destroy", i.push(r);
                    this.offlineData(n.concat(i)), e && (this._pristineData = this.reader.reader ? this.reader.reader._wrapDataAccessBase(n) : this.reader._wrapDataAccessBase(n))
                }
            },
            _addRange: function(e, n) {
                var i = this,
                    o = t !== n ? n : i._skip || 0,
                    r = o + i._flatData(e, !0).length;
                i._ranges.push({
                    start: o,
                    end: r,
                    data: e,
                    pristineData: e.toJSON(),
                    timestamp: i._timeStamp()
                }), i._sortRanges()
            },
            _sortRanges: function() {
                this._ranges.sort(function(e, t) {
                    return e.start - t.start
                })
            },
            error: function(e, t, n) {
                this._dequeueRequest(), this.trigger(Be, {}), this.trigger(Fe, {
                    xhr: e,
                    status: t,
                    errorThrown: n
                })
            },
            _params: function(e) {
                var t = this,
                    n = ue({
                        take: t.take(),
                        skip: t.skip(),
                        page: t.page(),
                        pageSize: t.pageSize(),
                        sort: t._sort,
                        filter: t._filter,
                        group: t._group,
                        aggregate: t._aggregate
                    }, e);
                return t.options.serverPaging || (delete n.take, delete n.skip, delete n.page, delete n.pageSize), t.options.serverGrouping ? t.reader.model && n.group && (n.group = N(n.group, t.reader.model)) : delete n.group, t.options.serverFiltering ? t.reader.model && n.filter && (n.filter = H(n.filter, t.reader.model)) : delete n.filter, t.options.serverSorting ? t.reader.model && n.sort && (n.sort = N(n.sort, t.reader.model)) : delete n.sort, t.options.serverAggregates ? t.reader.model && n.aggregate && (n.aggregate = N(n.aggregate, t.reader.model)) : delete n.aggregate, n
            },
            _queueRequest: function(e, n) {
                var i = this;
                i._requestInProgress ? i._pending = {
                    callback: he(n, i),
                    options: e
                } : (i._requestInProgress = !0, i._pending = t, n())
            },
            _dequeueRequest: function() {
                var e = this;
                e._requestInProgress = !1, e._pending && e._queueRequest(e._pending.options, e._pending.callback)
            },
            _handleCustomErrors: function(e) {
                if (this.reader.errors) {
                    var t = this.reader.errors(e);
                    if (t) return this.trigger(Fe, {
                        xhr: null,
                        status: "customerror",
                        errorThrown: "custom error",
                        errors: t
                    }), !0
                }
                return !1
            },
            _shouldWrap: function(e) {
                var t = this.reader.model;
                return !(!t || !e.length) && !(e[0] instanceof t)
            },
            _observe: function(e) {
                var t, n = this,
                    i = n.reader.model;
                return n._shouldDetachObservableParents = !0, e instanceof Ze ? (n._shouldDetachObservableParents = !1, n._shouldWrap(e) && (e.type = n.reader.model, e.wrapAll(e, e))) : (t = n.pageSize() && !n.options.serverPaging ? j : Ze, e = new t(e, n.reader.model), e.parent = function() {
                    return n.parent()
                }), n._isServerGrouped() && A(e, i), n._changeHandler && n._data && n._data instanceof Ze ? n._data.unbind(Ie, n._changeHandler) : n._changeHandler = he(n._change, n), e.bind(Ie, n._changeHandler)
            },
            _updateTotalForAction: function(e, t) {
                var n = this,
                    i = parseInt(n._total, 10);
                v(n._total) || (i = parseInt(n._pristineTotal, 10)), "add" === e ? i += t.length : "remove" === e ? i -= t.length : "itemchange" === e || "sync" === e || n.options.serverPaging ? "sync" === e && (i = n._pristineTotal = parseInt(n._total, 10)) : i = n._pristineTotal, n._total = i
            },
            _change: function(e) {
                var t, n, i, o = this,
                    r = e ? e.action : "";
                if ("remove" === r)
                    for (t = 0, n = e.items.length; t < n; t++) e.items[t].isNew && e.items[t].isNew() || o._destroyed.push(e.items[t]);
                !o.options.autoSync || "add" !== r && "remove" !== r && "itemchange" !== r ? (o._updateTotalForAction(r, e ? e.items : []), o._process(o._data, e)) : (i = function(t) {
                    "sync" === t.action && (o.unbind("change", i), o._updateTotalForAction(r, e.items))
                }, o.first("change", i), o.sync())
            },
            _calculateAggregates: function(e, t) {
                t = t || {};
                var n = new r(e),
                    i = t.aggregate,
                    o = t.filter;
                return o && (n = n.filter(o)), n.aggregate(i)
            },
            _process: function(e, n) {
                var i, o = this,
                    r = {};
                o.options.serverPaging !== !0 && (r.skip = o._skip, r.take = o._take || o._pageSize, r.skip === t && o._page !== t && o._pageSize !== t && (r.skip = (o._page - 1) * o._pageSize), o.options.useRanges && (r.skip = o.currentRangeStart())), o.options.serverSorting !== !0 && (r.sort = o._sort), o.options.serverFiltering !== !0 && (r.filter = o._filter), o.options.serverGrouping !== !0 && (r.group = o._group), o.options.serverAggregates !== !0 && (r.aggregate = o._aggregate), i = o._queryProcess(e, r), o.options.serverAggregates !== !0 && (o._aggregateResult = o._calculateAggregates(i.dataToAggregate || e, r)), o.view(i.data), o._setFilterTotal(i.total, !1), n = n || {}, n.items = n.items || o._view, o.trigger(Ie, n)
            },
            _queryProcess: function(e, t) {
                return this.options.inPlaceSort ? r.process(e, t, this.options.inPlaceSort) : r.process(e, t)
            },
            _mergeState: function(e) {
                var n = this;
                return e !== t && (n._pageSize = e.pageSize, n._page = e.page, n._sort = e.sort, n._filter = e.filter, n._group = e.group, n._aggregate = e.aggregate, n._skip = n._currentRangeStart = e.skip, n._take = e.take, n._skip === t && (n._skip = n._currentRangeStart = n.skip(), e.skip = n.skip()), n._take === t && n._pageSize !== t && (n._take = n._pageSize, e.take = n._take), e.sort && (n._sort = e.sort = s(e.sort)), e.filter && (n._filter = e.filter = l(e.filter)), e.group && (n._group = e.group = f(e.group)), e.aggregate && (n._aggregate = e.aggregate = p(e.aggregate))), e
            },
            query: function(n) {
                var i, o, r, s = this.options.serverSorting || this.options.serverPaging || this.options.serverFiltering || this.options.serverGrouping || this.options.serverAggregates;
                return s || (this._data === t || 0 === this._data.length) && !this._destroyed.length ? (this.options.endless && (o = n.pageSize - this.pageSize(), o > 0 ? (o = this.pageSize(), n.page = n.pageSize / o, n.pageSize = o) : (n.page = 1, this.options.endless = !1)), this.read(this._mergeState(n))) : (r = this.trigger(Pe, {
                    type: "read"
                }), r || (this.trigger(ze), i = this._queryProcess(this._data, this._mergeState(n)), this._setFilterTotal(i.total, !0), this._aggregateResult = this._calculateAggregates(i.dataToAggregate || this._data, n), this.view(i.data), this.trigger(Be, {
                    type: "read"
                }), this.trigger(Ie, {
                    items: i.data
                })), e.Deferred().resolve(r).promise())
            },
            _setFilterTotal: function(e, n) {
                var i = this;
                i.options.serverFiltering || (e !== t ? i._total = e : n && (i._total = i._data.length))
            },
            fetch: function(e) {
                var t = this,
                    n = function(n) {
                        n !== !0 && ye(e) && e.call(t)
                    };
                return this._query().then(n)
            },
            _query: function(e) {
                var t = this;
                return t.query(ue({}, {
                    page: t.page(),
                    pageSize: t.pageSize(),
                    sort: t.sort(),
                    filter: t.filter(),
                    group: t.group(),
                    aggregate: t.aggregate()
                }, e))
            },
            next: function(e) {
                var t = this,
                    n = t.page(),
                    i = t.total();
                if (e = e || {}, n && !(i && n + 1 > t.totalPages())) return t._skip = t._currentRangeStart = n * t.take(), n += 1, e.page = n, t._query(e), n
            },
            prev: function(e) {
                var t = this,
                    n = t.page();
                if (e = e || {}, n && 1 !== n) return t._skip = t._currentRangeStart = t._skip - t.take(), n -= 1, e.page = n, t._query(e), n
            },
            page: function(e) {
                var n, i = this;
                return e !== t ? (e = Ve.max(Ve.min(Ve.max(e, 1), i.totalPages()), 1), i._query(i._pageableQueryOptions({
                    page: e
                })), t) : (n = i.skip(), n !== t ? Ve.round((n || 0) / (i.take() || 1)) + 1 : t)
            },
            pageSize: function(e) {
                var n = this;
                return e !== t ? (n._query({
                    pageSize: e,
                    page: 1
                }), t) : n.take()
            },
            sort: function(e) {
                var n = this;
                return e !== t ? (n._query({
                    sort: e
                }), t) : n._sort
            },
            filter: function(e) {
                var n = this;
                return e === t ? n._filter : (n.trigger("reset"), n._query({
                    filter: e,
                    page: 1
                }), t)
            },
            group: function(e) {
                var n = this;
                return e !== t ? (n._query({
                    group: e
                }), t) : n._group
            },
            total: function() {
                return parseInt(this._total || 0, 10)
            },
            aggregate: function(e) {
                var n = this;
                return e !== t ? (n._query({
                    aggregate: e
                }), t) : n._aggregate
            },
            aggregates: function() {
                var e = this._aggregateResult;
                return fe(e) && (e = this._emptyAggregates(this.aggregate())), e
            },
            _emptyAggregates: function(e) {
                var t, n, i = {};
                if (!fe(e))
                    for (t = {}, me(e) || (e = [e]), n = 0; n < e.length; n++) t[e[n].aggregate] = 0, i[e[n].field] = t;
                return i
            },
            _pageableQueryOptions: function(e) {
                return e
            },
            _wrapInEmptyGroup: function(e) {
                var t, n, i, o, r = this.group();
                for (i = r.length - 1, o = 0; i >= o; i--) n = r[i], t = {
                    value: e.get(n.field),
                    field: n.field,
                    items: t ? [t] : [e],
                    hasSubgroups: !!t,
                    aggregates: this._emptyAggregates(n.aggregates)
                };
                return t
            },
            totalPages: function() {
                var e = this,
                    t = e.pageSize() || e.total();
                return Ve.ceil((e.total() || 0) / t)
            },
            inRange: function(e, t) {
                var n = this,
                    i = Ve.min(e + t, n.total());
                return !n.options.serverPaging && n._data.length > 0 || n._findRange(e, i).length > 0
            },
            lastRange: function() {
                var e = this._ranges;
                return e[e.length - 1] || {
                    start: 0,
                    end: 0,
                    data: []
                }
            },
            firstItemUid: function() {
                var e = this._ranges;
                return e.length && e[0].data.length && e[0].data[0].uid
            },
            enableRequestsInProgress: function() {
                this._skipRequestsInProgress = !1
            },
            _timeStamp: function() {
                return (new Date).getTime()
            },
            range: function(e, n, i) {
                this._currentRequestTimeStamp = this._timeStamp(), this._skipRequestsInProgress = !0, e = Ve.min(e || 0, this.total()), i = ye(i) ? i : be;
                var o, r = this,
                    s = Ve.max(Ve.floor(e / n), 0) * n,
                    a = Ve.min(s + n, r.total());
                return o = r._findRange(e, Ve.min(e + n, r.total())), o.length || 0 === r.total() ? (r._processRangeData(o, e, n, s, a), i(), t) : (n !== t && (r._rangeExists(s, a) ? s < e && r.prefetch(a, n, function() {
                    r.range(e, n, i)
                }) : r.prefetch(s, n, function() {
                    e > s && a < r.total() && !r._rangeExists(a, Ve.min(a + n, r.total())) ? r.prefetch(a, n, function() {
                        r.range(e, n, i)
                    }) : r.range(e, n, i)
                })), t)
            },
            _findRange: function(e, n) {
                var i, o, r, a, l, c, d, u, h, p, m, g, v = this,
                    _ = v._ranges,
                    b = [],
                    w = v.options,
                    y = w.serverSorting || w.serverPaging || w.serverFiltering || w.serverGrouping || w.serverAggregates;
                for (o = 0, m = _.length; o < m; o++)
                    if (i = _[o], e >= i.start && e <= i.end) {
                        for (p = 0, r = o; r < m; r++)
                            if (i = _[r], h = v._flatData(i.data, !0), h.length && e + p >= i.start && (c = i.data, d = i.end, y || (w.inPlaceSort ? u = v._queryProcess(i.data, {
                                    filter: v.filter()
                                }) : (g = f(v.group() || []).concat(s(v.sort() || [])), u = v._queryProcess(i.data, {
                                    sort: g,
                                    filter: v.filter()
                                })), h = c = u.data, u.total !== t && (d = u.total)), a = 0, e + p > i.start && (a = e + p - i.start), l = h.length, d > n && (l -= d - n), p += l - a, b = v._mergeGroups(b, c, a, l), n <= i.end && p == n - e)) return b;
                        break
                    }
                return []
            },
            _mergeGroups: function(e, t, n, i) {
                if (this._isServerGrouped()) {
                    var o, r = t.toJSON();
                    return e.length && (o = e[e.length - 1]), T(o, r, n, i), e.concat(r)
                }
                return e.concat(t.slice(n, i))
            },
            _processRangeData: function(e, n, i, o, r) {
                var s, a, l, c, d = this;
                d._pending = t, d._skip = n > d.skip() ? Ve.min(r, (d.totalPages() - 1) * d.take()) : o, d._currentRangeStart = n, d._take = i, s = d.options.serverPaging, a = d.options.serverSorting, l = d.options.serverFiltering, c = d.options.serverAggregates;
                try {
                    d.options.serverPaging = !0, d._isServerGrouped() || d.group() && d.group().length || (d.options.serverSorting = !0), d.options.serverFiltering = !0, d.options.serverPaging = !0, d.options.serverAggregates = !0, s && (d._detachObservableParents(), d._data = e = d._observe(e)), d._process(e)
                } finally {
                    d.options.serverPaging = s, d.options.serverSorting = a, d.options.serverFiltering = l, d.options.serverAggregates = c
                }
            },
            skip: function() {
                var e = this;
                return e._skip === t ? e._page !== t ? (e._page - 1) * (e.take() || 1) : t : e._skip
            },
            currentRangeStart: function() {
                return this._currentRangeStart || 0
            },
            take: function() {
                return this._take || this._pageSize
            },
            _prefetchSuccessHandler: function(e, t, n, i) {
                var o = this,
                    r = o._timeStamp();
                return function(s) {
                    var a, l, c, d = !1,
                        u = {
                            start: e,
                            end: t,
                            data: [],
                            timestamp: o._timeStamp()
                        };
                    if (o._dequeueRequest(), o.trigger(Be, {
                            response: s,
                            type: "read"
                        }), s = o.reader.parse(s), c = o._readData(s), c.length) {
                        for (a = 0, l = o._ranges.length; a < l; a++)
                            if (o._ranges[a].start === e) {
                                d = !0, u = o._ranges[a], u.pristineData = c, u.data = o._observe(c), u.end = u.start + o._flatData(u.data, !0).length, o._sortRanges();
                                break
                            }
                        d || o._addRange(o._observe(c), e)
                    }
                    o._total = o.reader.total(s), (i || r >= o._currentRequestTimeStamp || !o._skipRequestsInProgress) && (n && c.length ? n() : o.trigger(Ie, {}))
                }
            },
            prefetch: function(e, t, n) {
                var i = this,
                    o = Ve.min(e + t, i.total()),
                    r = {
                        take: t,
                        skip: e,
                        page: e / t + 1,
                        pageSize: t,
                        sort: i._sort,
                        filter: i._filter,
                        group: i._group,
                        aggregate: i._aggregate
                    };
                i._rangeExists(e, o) ? n && n() : (clearTimeout(i._timeout), i._timeout = setTimeout(function() {
                    i._queueRequest(r, function() {
                        i.trigger(Pe, {
                            type: "read"
                        }) ? i._dequeueRequest() : i.transport.read({
                            data: i._params(r),
                            success: i._prefetchSuccessHandler(e, o, n),
                            error: function() {
                                var e = $e.call(arguments);
                                i.error.apply(i, e)
                            }
                        })
                    })
                }, 100))
            },
            _multiplePrefetch: function(e, t, n) {
                var i = this,
                    o = Ve.min(e + t, i.total()),
                    r = {
                        take: t,
                        skip: e,
                        page: e / t + 1,
                        pageSize: t,
                        sort: i._sort,
                        filter: i._filter,
                        group: i._group,
                        aggregate: i._aggregate
                    };
                i._rangeExists(e, o) ? n && n() : i.trigger(Pe, {
                    type: "read"
                }) || i.transport.read({
                    data: i._params(r),
                    success: i._prefetchSuccessHandler(e, o, n, !0)
                })
            },
            _rangeExists: function(e, t) {
                var n, i, o = this,
                    r = o._ranges;
                for (n = 0, i = r.length; n < i; n++)
                    if (r[n].start <= e && r[n].end >= t) return !0;
                return !1
            },
            _getCurrentRangeSpan: function() {
                var e, t, n = this,
                    i = n._ranges,
                    o = n.currentRangeStart(),
                    r = o + (n.take() || 0),
                    s = [],
                    a = i.length;
                for (t = 0; t < a; t++) e = i[t], (e.start <= o && e.end >= o || e.start >= o && e.start <= r) && s.push(e);
                return s
            },
            _removeModelFromRanges: function(e) {
                var t, n, i, o, r = this;
                for (i = 0, o = this._ranges.length; i < o && (n = this._ranges[i], this._eachItem(n.data, function(n) {
                        t = F(n, e)
                    }), !t); i++);
                r._updateRangesLength()
            },
            _insertModelInRange: function(e, t) {
                var n, i, o = this,
                    r = o._ranges || [],
                    s = r.length;
                for (i = 0; i < s; i++)
                    if (n = r[i], n.start <= e && n.end >= e) {
                        o._getByUid(t.uid, n.data) || (o._isServerGrouped() ? n.data.splice(e, 0, o._wrapInEmptyGroup(t)) : n.data.splice(e, 0, t));
                        break
                    }
                o._updateRangesLength()
            },
            _updateRangesLength: function() {
                var e, t, n = this,
                    i = n._ranges || [],
                    o = i.length,
                    r = !1,
                    s = 0,
                    a = 0;
                for (t = 0; t < o; t++) e = i[t], a = n._flatData(e.data, !0).length - Ve.abs(e.end - e.start), r || 0 === a ? r && (e.start += s, e.end += s) : (r = !0, s = a, e.end += s)
            }
        }), se = {}, se.create = function(t, n, i) {
            var o, r = t.transport ? e.extend({}, t.transport) : null;
            return r ? (r.read = typeof r.read === Ce ? {
                url: r.read
            } : r.read, "jsdo" === t.type && (r.dataSource = i), t.type && (we.data.transports = we.data.transports || {}, we.data.schemas = we.data.schemas || {}, we.data.transports[t.type] ? pe(we.data.transports[t.type]) ? r = ue(!0, {}, we.data.transports[t.type], r) : o = new we.data.transports[t.type](ue(r, {
                data: n
            })) : we.logToConsole("Unknown DataSource transport type '" + t.type + "'.\nVerify that registration scripts for this type are included after Kendo UI on the page.", "warn"), t.schema = ue(!0, {}, we.data.schemas[t.type], t.schema)), o || (o = ye(r.read) ? r : new ne(r))) : o = new te({
                data: t.data || []
            }), o
        }, re.create = function(e) {
            (me(e) || e instanceof Ze) && (e = {
                data: e
            });
            var n, i, o, r = e || {},
                s = r.data,
                a = r.fields,
                l = r.table,
                c = r.select,
                d = {};
            if (s || !a || r.transport || (l ? s = V(l, a) : c && (s = O(c, a), r.group === t && s[0] && s[0].optgroup !== t && (r.group = "optgroup"))), we.data.Model && a && (!r.schema || !r.schema.model)) {
                for (n = 0, i = a.length; n < i; n++) o = a[n], o.type && (d[o.field] = o);
                fe(d) || (r.schema = ue(!0, r.schema, {
                    model: {
                        fields: d
                    }
                }))
            }
            return r.data = s, c = null, r.select = null, l = null, r.table = null, r instanceof re ? r : new re(r)
        }, ae = Y.define({
            idField: "id",
            init: function(e) {
                var t, n = this,
                    i = n.hasChildren || e && e.hasChildren,
                    o = "items",
                    r = {};
                we.data.Model.fn.init.call(n, e), typeof n.children === Ce && (o = n.children), r = {
                    schema: {
                        data: o,
                        model: {
                            hasChildren: i,
                            id: n.idField,
                            fields: n.fields
                        }
                    }
                }, typeof n.children !== Ce && ue(r, n.children), r.data = e, i || (i = r.schema.data), typeof i === Ce && (i = we.getter(i)), ye(i) && (t = i.call(n, n), n.hasChildren = (!t || 0 !== t.length) && !!t), n._childrenOptions = r, n.hasChildren && n._initChildren(), n._loaded = !(!e || !e._loaded)
            },
            _initChildren: function() {
                var e, t, n, i = this;
                i.children instanceof le || (e = i.children = new le(i._childrenOptions), t = e.transport, n = t.parameterMap, t.parameterMap = function(e, t) {
                    return e[i.idField || "id"] = i.id, n && (e = n(e, t)), e
                }, e.parent = function() {
                    return i
                }, e.bind(Ie, function(e) {
                    e.node = e.node || i, i.trigger(Ie, e)
                }), e.bind(Fe, function(e) {
                    var t = i.parent();
                    t && (e.node = e.node || i, t.trigger(Fe, e))
                }), i._updateChildrenField())
            },
            append: function(e) {
                this._initChildren(), this.loaded(!0), this.children.add(e)
            },
            hasChildren: !1,
            level: function() {
                for (var e = this.parentNode(), t = 0; e && e.parentNode;) t++, e = e.parentNode ? e.parentNode() : null;
                return t
            },
            _updateChildrenField: function() {
                var e = this._childrenOptions.schema.data;
                this[e || "items"] = this.children.data()
            },
            _childrenLoaded: function() {
                this._loaded = !0, this._updateChildrenField()
            },
            load: function() {
                var n, i, o = {},
                    r = "_query";
                return this.hasChildren ? (this._initChildren(), n = this.children, o[this.idField || "id"] = this.id, this._loaded || (n._data = t, r = "read"), n.one(Ie, he(this._childrenLoaded, this)), this._matchFilter && (o.filter = {
                    field: "_matchFilter",
                    operator: "eq",
                    value: !0
                }), i = n[r](o)) : this.loaded(!0), i || e.Deferred().resolve().promise()
            },
            parentNode: function() {
                var e = this.parent();
                return e.parent()
            },
            loaded: function(e) {
                return e === t ? this._loaded : (this._loaded = e, t)
            },
            shouldSerialize: function(e) {
                return Y.fn.shouldSerialize.call(this, e) && "children" !== e && "_loaded" !== e && "hasChildren" !== e && "_childrenOptions" !== e
            }
        }), le = re.extend({
            init: function(e) {
                var t = ae.define({
                    children: e
                });
                e.filter && !e.serverFiltering && (this._hierarchicalFilter = e.filter, e.filter = null), re.fn.init.call(this, ue(!0, {}, {
                    schema: {
                        modelBase: t,
                        model: t
                    }
                }, e)), this._attachBubbleHandlers()
            },
            _attachBubbleHandlers: function() {
                var e = this;
                e._data.bind(Fe, function(t) {
                    e.trigger(Fe, t)
                })
            },
            read: function(e) {
                var t = re.fn.read.call(this, e);
                return this._hierarchicalFilter && (this._data && this._data.length > 0 ? this.filter(this._hierarchicalFilter) : (this.options.filter = this._hierarchicalFilter, this._filter = l(this.options.filter), this._hierarchicalFilter = null)), t
            },
            remove: function(e) {
                var t, n = e.parentNode(),
                    i = this;
                return n && n._initChildren && (i = n.children), t = re.fn.remove.call(i, e), n && !i.data().length && (n.hasChildren = !1), t
            },
            success: W("success"),
            data: W("data"),
            insert: function(e, t) {
                var n = this.parent();
                return n && n._initChildren && (n.hasChildren = !0, n._initChildren()), re.fn.insert.call(this, e, t)
            },
            filter: function(e) {
                return e === t ? this._filter : (!this.options.serverFiltering && this._markHierarchicalQuery(e) && (e = {
                    logic: "or",
                    filters: [e, {
                        field: "_matchFilter",
                        operator: "equals",
                        value: !0
                    }]
                }), this.trigger("reset"), this._query({
                    filter: e,
                    page: 1
                }), t)
            },
            _markHierarchicalQuery: function(e) {
                var t, n, i, o, s;
                return e = l(e), e && 0 !== e.filters.length ? (t = r.filterExpr(e), i = t.fields, o = t.operators, n = s = Function("d, __f, __o", "return " + t.expression), (i.length || o.length) && (s = function(e) {
                    return n(e, i, o)
                }), this._updateHierarchicalFilter(s), !0) : (this._updateHierarchicalFilter(function() {
                    return !0
                }), !1)
            },
            _updateHierarchicalFilter: function(e) {
                var t, n, i = this._data,
                    o = !1;
                for (n = 0; n < i.length; n++) t = i[n], t.hasChildren ? (t._matchFilter = t.children._updateHierarchicalFilter(e), t._matchFilter || (t._matchFilter = e(t))) : t._matchFilter = e(t), t._matchFilter && (o = !0);
                return o
            },
            _find: function(e, t) {
                var n, i, o, r, s = this._data;
                if (s) {
                    if (o = re.fn[e].call(this, t)) return o;
                    for (s = this._flatData(this._data), n = 0, i = s.length; n < i; n++)
                        if (r = s[n].children, r instanceof le && (o = r[e](t))) return o
                }
            },
            get: function(e) {
                return this._find("get", e)
            },
            getByUid: function(e) {
                return this._find("getByUid", e)
            }
        }), le.create = function(e) {
            e = e && e.push ? {
                data: e
            } : e;
            var t = e || {},
                n = t.data,
                i = t.fields,
                o = t.list;
            return n && n._dataSource ? n._dataSource : (n || !i || t.transport || o && (n = U(o, i)), t.data = n, t instanceof le ? t : new le(t))
        }, ce = we.Observable.extend({
            init: function(e, t, n) {
                we.Observable.fn.init.call(this), this._prefetching = !1, this.dataSource = e, this.prefetch = !n;
                var i = this;
                e.bind("change", function() {
                    i._change()
                }), e.bind("reset", function() {
                    i._reset()
                }), this._syncWithDataSource(), this.setViewSize(t)
            },
            setViewSize: function(e) {
                this.viewSize = e, this._recalculate()
            },
            at: function(e) {
                var n = this.pageSize,
                    i = !0;
                return e >= this.total() ? (this.trigger("endreached", {
                    index: e
                }), null) : this.useRanges ? this.useRanges ? ((e < this.dataOffset || e >= this.skip + n) && (i = this.range(Math.floor(e / n) * n)), e === this.prefetchThreshold && this._prefetch(), e === this.midPageThreshold ? this.range(this.nextMidRange, !0) : e === this.nextPageThreshold ? this.range(this.nextFullRange) : e === this.pullBackThreshold && this.range(this.offset === this.skip ? this.previousMidRange : this.previousFullRange), i ? this.dataSource.at(e - this.dataOffset) : (this.trigger("endreached", {
                    index: e
                }), null)) : t : this.dataSource.view()[e]
            },
            indexOf: function(e) {
                return this.dataSource.data().indexOf(e) + this.dataOffset
            },
            total: function() {
                return parseInt(this.dataSource.total(), 10)
            },
            next: function() {
                var e = this,
                    t = e.pageSize,
                    n = e.skip - e.viewSize + t,
                    i = Ve.max(Ve.floor(n / t), 0) * t;
                this.offset = n, this.dataSource.prefetch(i, t, function() {
                    e._goToRange(n, !0)
                })
            },
            range: function(e, t) {
                if (this.offset === e) return !0;
                var n = this,
                    i = this.pageSize,
                    o = Ve.max(Ve.floor(e / i), 0) * i,
                    r = this.dataSource;
                return t && (o += i), r.inRange(e, i) ? (this.offset = e, this._recalculate(), this._goToRange(e), !0) : !this.prefetch || (r.prefetch(o, i, function() {
                    n.offset = e, n._recalculate(), n._goToRange(e, !0)
                }), !1)
            },
            syncDataSource: function() {
                var e = this.offset;
                this.offset = null, this.range(e)
            },
            destroy: function() {
                this.unbind()
            },
            _prefetch: function() {
                var e = this,
                    t = this.pageSize,
                    n = this.skip + t,
                    i = this.dataSource;
                i.inRange(n, t) || this._prefetching || !this.prefetch || (this._prefetching = !0, this.trigger("prefetching", {
                    skip: n,
                    take: t
                }), i.prefetch(n, t, function() {
                    e._prefetching = !1, e.trigger("prefetched", {
                        skip: n,
                        take: t
                    })
                }))
            },
            _goToRange: function(e, t) {
                this.offset === e && (this.dataOffset = e, this._expanding = t, this.dataSource.range(e, this.pageSize), this.dataSource.enableRequestsInProgress())
            },
            _reset: function() {
                this._syncPending = !0
            },
            _change: function() {
                var e = this.dataSource;
                this.length = this.useRanges ? e.lastRange().end : e.view().length, this._syncPending && (this._syncWithDataSource(), this._recalculate(), this._syncPending = !1, this.trigger("reset", {
                    offset: this.offset
                })), this.trigger("resize"), this._expanding && this.trigger("expand"), delete this._expanding
            },
            _syncWithDataSource: function() {
                var e = this.dataSource;
                this._firstItemUid = e.firstItemUid(), this.dataOffset = this.offset = e.skip() || 0, this.pageSize = e.pageSize(), this.useRanges = e.options.serverPaging
            },
            _recalculate: function() {
                var e = this.pageSize,
                    t = this.offset,
                    n = this.viewSize,
                    i = Math.ceil(t / e) * e;
                this.skip = i, this.midPageThreshold = i + e - 1, this.nextPageThreshold = i + n - 1, this.prefetchThreshold = i + Math.floor(e / 3 * 2), this.pullBackThreshold = this.offset - 1, this.nextMidRange = i + e - n, this.nextFullRange = i, this.previousMidRange = t - n, this.previousFullRange = i - e
            }
        }), de = we.Observable.extend({
            init: function(e, t) {
                var n = this;
                we.Observable.fn.init.call(n), this.dataSource = e, this.batchSize = t, this._total = 0, this.buffer = new ce(e, 3 * t), this.buffer.bind({
                    endreached: function(e) {
                        n.trigger("endreached", {
                            index: e.index
                        })
                    },
                    prefetching: function(e) {
                        n.trigger("prefetching", {
                            skip: e.skip,
                            take: e.take
                        })
                    },
                    prefetched: function(e) {
                        n.trigger("prefetched", {
                            skip: e.skip,
                            take: e.take
                        })
                    },
                    reset: function() {
                        n._total = 0, n.trigger("reset")
                    },
                    resize: function() {
                        n._total = Math.ceil(this.length / n.batchSize), n.trigger("resize", {
                            total: n.total(),
                            offset: this.offset
                        })
                    }
                })
            },
            syncDataSource: function() {
                this.buffer.syncDataSource()
            },
            at: function(e) {
                var t, n, i = this.buffer,
                    o = e * this.batchSize,
                    r = this.batchSize,
                    s = [];
                for (i.offset > o && i.at(i.offset - 1), n = 0; n < r && (t = i.at(o + n), null !== t); n++) s.push(t);
                return s
            },
            total: function() {
                return this._total
            },
            destroy: function() {
                this.buffer.destroy(), this.unbind()
            }
        }), ue(!0, we.data, {
            readers: {
                json: oe
            },
            Query: r,
            DataSource: re,
            HierarchicalDataSource: le,
            Node: ae,
            ObservableObject: G,
            ObservableArray: Ze,
            LazyObservableArray: j,
            LocalTransport: te,
            RemoteTransport: ne,
            Cache: ie,
            DataReader: oe,
            Model: Y,
            Buffer: ce,
            BatchBuffer: de
        })
    }(window.kendo.jQuery), window.kendo
}, "function" == typeof define && define.amd ? define : function(e, t, n) {
    (n || t)()
}),

function(e, define) {
    define("kendo.popup.min", ["kendo.core.min"], e)
}(function() {
    return function(e, t) {
        function n(t, n) {
            return !(!t || !n) && (t === n || e.contains(t, n))
        }
        var i, o, r, s, a = window.kendo,
            l = a.ui,
            c = l.Widget,
            d = a.Class,
            u = a.support,
            h = a.getOffset,
            p = a._outerWidth,
            f = a._outerHeight,
            m = "open",
            g = "close",
            v = "deactivate",
            _ = "activate",
            b = "center",
            w = "left",
            y = "right",
            k = "top",
            x = "bottom",
            C = "absolute",
            S = "hidden",
            T = "body",
            D = "location",
            A = "position",
            E = "visible",
            I = "effects",
            M = "k-state-active",
            R = "k-state-border",
            F = /k-state-border-(\w+)/,
            P = ".k-picker-wrap, .k-dropdown-wrap, .k-link",
            z = "down",
            B = e(document.documentElement),
            L = e.proxy,
            H = e(window),
            N = "scroll",
            O = u.transitions.css,
            V = O + "transform",
            W = e.extend,
            U = ".kendoPopup",
            q = ["font-size", "font-family", "font-stretch", "font-style", "font-weight", "line-height"],
            j = c.extend({
                init: function(t, n) {
                    var i, o = this;
                    n = n || {}, n.isRtl && (n.origin = n.origin || x + " " + y, n.position = n.position || k + " " + y), c.fn.init.call(o, t, n), t = o.element, n = o.options, o.collisions = n.collision ? n.collision.split(" ") : [], o.downEvent = a.applyEventMap(z, a.guid()), 1 === o.collisions.length && o.collisions.push(o.collisions[0]), i = e(o.options.anchor).closest(".k-popup,.k-group").filter(":not([class^=km-])"), n.appendTo = e(e(n.appendTo)[0] || i[0] || document.body), o.element.hide().addClass("k-popup k-group k-reset").toggleClass("k-rtl", !!n.isRtl).css({
                        position: C
                    }).appendTo(n.appendTo).attr("aria-hidden", !0).on("mouseenter" + U, function() {
                        o._hovered = !0
                    }).on("wheel" + U, function(t) {
                        var n = e(t.target).find(".k-list"),
                            i = n.parent();
                        n.length && n.is(":visible") && (0 === i.scrollTop() && t.originalEvent.deltaY < 0 || i.scrollTop() === i.prop("scrollHeight") - i.prop("offsetHeight") && t.originalEvent.deltaY > 0) && t.preventDefault()
                    }).on("mouseleave" + U, function() {
                        o._hovered = !1
                    }), o.wrapper = e(), n.animation === !1 && (n.animation = {
                        open: {
                            effects: {}
                        },
                        close: {
                            hide: !0,
                            effects: {}
                        }
                    }), W(n.animation.open, {
                        complete: function() {
                            o.wrapper.css({
                                overflow: E
                            }), o._activated = !0, o._trigger(_)
                        }
                    }), W(n.animation.close, {
                        complete: function() {
                            o._animationClose()
                        }
                    }), o._mousedownProxy = function(e) {
                        o._mousedown(e)
                    }, o._resizeProxy = u.mobileOS.android ? function(e) {
                        setTimeout(function() {
                            o._resize(e)
                        }, 600)
                    } : function(e) {
                        o._resize(e)
                    }, n.toggleTarget && e(n.toggleTarget).on(n.toggleEvent + U, e.proxy(o.toggle, o))
                },
                events: [m, _, g, v],
                options: {
                    name: "Popup",
                    toggleEvent: "click",
                    origin: x + " " + w,
                    position: k + " " + w,
                    anchor: T,
                    appendTo: null,
                    collision: "flip fit",
                    viewport: window,
                    copyAnchorStyles: !0,
                    autosize: !1,
                    modal: !1,
                    adjustSize: {
                        width: 0,
                        height: 0
                    },
                    animation: {
                        open: {
                            effects: "slideIn:down",
                            transition: !0,
                            duration: 200
                        },
                        close: {
                            duration: 100,
                            hide: !0
                        }
                    }
                },
                _animationClose: function() {
                    var e = this,
                        t = e.wrapper.data(D);
                    e.wrapper.hide(), t && e.wrapper.css(t), e.options.anchor != T && e._hideDirClass(), e._closing = !1, e._trigger(v)
                },
                destroy: function() {
                    var t, n = this,
                        i = n.options,
                        o = n.element.off(U);
                    c.fn.destroy.call(n), i.toggleTarget && e(i.toggleTarget).off(U), i.modal || (B.unbind(n.downEvent, n._mousedownProxy), n._toggleResize(!1)), a.destroy(n.element.children()), o.removeData(), i.appendTo[0] === document.body && (t = o.parent(".k-animation-container"), t[0] ? t.remove() : o.remove())
                },
                open: function(t, n) {
                    var i, o, r = this,
                        s = {
                            isFixed: !isNaN(parseInt(n, 10)),
                            x: t,
                            y: n
                        },
                        l = r.element,
                        c = r.options,
                        d = e(c.anchor),
                        h = l[0] && l.hasClass("km-widget");
                    if (!r.visible()) {
                        if (c.copyAnchorStyles && (h && "font-size" == q[0] && q.shift(), l.css(a.getComputedStyles(d[0], q))), l.data("animating") || r._trigger(m)) return;
                        r._activated = !1, c.modal || (B.unbind(r.downEvent, r._mousedownProxy).bind(r.downEvent, r._mousedownProxy), r._toggleResize(!1), r._toggleResize(!0)), r.wrapper = o = a.wrap(l, c.autosize).css({
                            overflow: S,
                            display: "block",
                            position: C
                        }).attr("aria-hidden", !1), u.mobileOS.android && o.css(V, "translatez(0)"), o.css(A), e(c.appendTo)[0] == document.body && o.css(k, "-10000px"), r.flipped = r._position(s), i = r._openAnimation(), c.anchor != T && r._showDirClass(i), l.data(I, i.effects).kendoStop(!0).kendoAnimate(i).attr("aria-hidden", !1)
                    }
                },
                _location: function(t) {
                    var n, i, o = this,
                        r = o.element,
                        s = o.options,
                        l = e(s.anchor),
                        c = r[0] && r.hasClass("km-widget");
                    return s.copyAnchorStyles && (c && "font-size" == q[0] && q.shift(), r.css(a.getComputedStyles(l[0], q))), o.wrapper = n = a.wrap(r, s.autosize).css({
                        overflow: S,
                        display: "block",
                        position: C
                    }), u.mobileOS.android && n.css(V, "translatez(0)"), n.css(A), e(s.appendTo)[0] == document.body && n.css(k, "-10000px"), o._position(t || {}), i = n.offset(), {
                        width: a._outerWidth(n),
                        height: a._outerHeight(n),
                        left: i.left,
                        top: i.top
                    }
                },
                _openAnimation: function() {
                    var e = W(!0, {}, this.options.animation.open);
                    return e.effects = a.parseEffects(e.effects, this.flipped), e
                },
                _hideDirClass: function() {
                    var t = e(this.options.anchor),
                        n = ((t.attr("class") || "").match(F) || ["", "down"])[1],
                        i = R + "-" + n;
                    t.removeClass(i).children(P).removeClass(M).removeClass(i), this.element.removeClass(R + "-" + a.directions[n].reverse)
                },
                _showDirClass: function(t) {
                    var n = t.effects.slideIn ? t.effects.slideIn.direction : "down",
                        i = R + "-" + n;
                    e(this.options.anchor).addClass(i).children(P).addClass(M).addClass(i), this.element.addClass(R + "-" + a.directions[n].reverse)
                },
                position: function() {
                    this.visible() && (this.flipped = this._position())
                },
                toggle: function() {
                    var e = this;
                    e[e.visible() ? g : m]()
                },
                visible: function() {
                    return this.element.is(":" + E)
                },
                close: function(n) {
                    var i, o, r, s, l = this,
                        c = l.options;
                    if (l.visible()) {
                        if (i = l.wrapper[0] ? l.wrapper : a.wrap(l.element).hide(), l._toggleResize(!1), l._closing || l._trigger(g)) return l._toggleResize(!0), t;
                        l.element.find(".k-popup").each(function() {
                            var t = e(this),
                                i = t.data("kendoPopup");
                            i && i.close(n)
                        }), B.unbind(l.downEvent, l._mousedownProxy), n ? o = {
                            hide: !0,
                            effects: {}
                        } : (o = W(!0, {}, c.animation.close), r = l.element.data(I), s = o.effects, !s && !a.size(s) && r && a.size(r) && (o.effects = r, o.reverse = !0), l._closing = !0), l.element.kendoStop(!0).attr("aria-hidden", !0), i.css({
                            overflow: S
                        }).attr("aria-hidden", !0), l.element.kendoAnimate(o), n && l._animationClose()
                    }
                },
                _trigger: function(e) {
                    return this.trigger(e, {
                        type: e
                    })
                },
                _resize: function(e) {
                    var t = this;
                    u.resize.indexOf(e.type) !== -1 ? (clearTimeout(t._resizeTimeout), t._resizeTimeout = setTimeout(function() {
                        t._position(), t._resizeTimeout = null
                    }, 50)) : (!t._hovered || t._activated && t.element.hasClass("k-list-container")) && t.close()
                },
                _toggleResize: function(e) {
                    var t = e ? "on" : "off",
                        n = u.resize;
                    u.mobileOS.ios || u.mobileOS.android || (n += " " + N), this._scrollableParents()[t](N, this._resizeProxy), H[t](n, this._resizeProxy)
                },
                _mousedown: function(t) {
                    var i = this,
                        o = i.element[0],
                        r = i.options,
                        s = e(r.anchor)[0],
                        l = r.toggleTarget,
                        c = a.eventTarget(t),
                        d = e(c).closest(".k-popup"),
                        u = d.parent().parent(".km-shim").length;
                    d = d[0], !u && d && d !== i.element[0] || "popover" !== e(t.target).closest("a").data("rel") && (n(o, c) || n(s, c) || l && n(e(l)[0], c) || i.close())
                },
                _fit: function(e, t, n) {
                    var i = 0;
                    return e + t > n && (i = n - (e + t)), e < 0 && (i = -e), i
                },
                _flip: function(e, t, n, i, o, r, s) {
                    var a = 0;
                    return s = s || t, r !== o && r !== b && o !== b && (e + s > i && (a += -(n + t)), e + a < 0 && (a += n + t)), a
                },
                _scrollableParents: function() {
                    return e(this.options.anchor).parentsUntil("body").filter(function(e, t) {
                        return a.isScrollable(t)
                    })
                },
                _position: function(t) {
                    var n, i, o, r, s, l, c, d, m, g, v, _, b, w, y, k, x, S = this,
                        T = S.element,
                        E = S.wrapper,
                        I = S.options,
                        M = e(I.viewport),
                        R = u.zoomLevel(),
                        F = !!(M[0] == window && window.innerWidth && R <= 1.02),
                        P = e(I.anchor),
                        z = I.origin.toLowerCase().split(" "),
                        B = I.position.toLowerCase().split(" "),
                        L = S.collisions,
                        H = 10002,
                        N = 0,
                        O = document.documentElement;
                    if (s = I.viewport === window ? {
                            top: window.pageYOffset || document.documentElement.scrollTop || 0,
                            left: window.pageXOffset || document.documentElement.scrollLeft || 0
                        } : M.offset(), F ? (l = window.innerWidth, c = window.innerHeight) : (l = M.width(), c = M.height()), F && O.scrollHeight - O.clientHeight > 0 && (d = I.isRtl ? -1 : 1, l -= d * a.support.scrollbar()), n = P.parents().filter(E.siblings()), n[0])
                        if (o = Math.max(+n.css("zIndex"), 0)) H = o + 10;
                        else
                            for (i = P.parentsUntil(n), r = i.length; N < r; N++) o = +e(i[N]).css("zIndex"), o && H < o && (H = o + 10);
                    return E.css("zIndex", H), E.css(t && t.isFixed ? {
                        left: t.x,
                        top: t.y
                    } : S._align(z, B)), m = h(E, A, P[0] === E.offsetParent()[0]), g = h(E), v = P.offsetParent().parent(".k-animation-container,.k-popup,.k-group"), v.length && (m = h(E, A, !0), g = h(E)), g.top -= s.top, g.left -= s.left, S.wrapper.data(D) || E.data(D, W({}, m)), _ = W({}, g), b = W({}, m), w = I.adjustSize, "fit" === L[0] && (b.top += S._fit(_.top, f(E) + w.height, c / R)), "fit" === L[1] && (b.left += S._fit(_.left, p(E) + w.width, l / R)), y = W({}, b), k = f(T), x = f(E), !E.height() && k && (x += k), "flip" === L[0] && (b.top += S._flip(_.top, k, f(P), c / R, z[0], B[0], x)), "flip" === L[1] && (b.left += S._flip(_.left, p(T), p(P), l / R, z[1], B[1], p(E))), T.css(A, C), E.css(b), b.left != y.left || b.top != y.top
                },
                _align: function(t, n) {
                    var i, o = this,
                        r = o.wrapper,
                        s = e(o.options.anchor),
                        a = t[0],
                        l = t[1],
                        c = n[0],
                        d = n[1],
                        u = h(s),
                        m = e(o.options.appendTo),
                        g = p(r),
                        v = f(r) || f(r.children().first()),
                        _ = p(s),
                        w = f(s),
                        k = u.top,
                        C = u.left,
                        S = Math.round;
                    return m[0] != document.body && (i = h(m), k -= i.top, C -= i.left), a === x && (k += w), a === b && (k += S(w / 2)), c === x && (k -= v), c === b && (k -= S(v / 2)), l === y && (C += _), l === b && (C += S(_ / 2)), d === y && (C -= g), d === b && (C -= S(g / 2)), {
                        top: k,
                        left: C
                    }
                }
            });
        l.plugin(j), i = a.support.stableSort, o = "kendoTabKeyTrap", r = "a[href], area[href], input:not([disabled]), select:not([disabled]), textarea:not([disabled]), button:not([disabled]), iframe, object, embed, [tabindex], *[contenteditable]", s = d.extend({
            init: function(t) {
                this.element = e(t), this.element.autoApplyNS(o)
            },
            trap: function() {
                this.element.on("keydown", L(this._keepInTrap, this))
            },
            removeTrap: function() {
                this.element.kendoDestroy(o)
            },
            destroy: function() {
                this.element.kendoDestroy(o), this.element = t
            },
            shouldTrap: function() {
                return !0
            },
            _keepInTrap: function(e) {
                var t, n, i;
                9 === e.which && this.shouldTrap() && !e.isDefaultPrevented() && (t = this._focusableElements(), n = this._sortFocusableElements(t), i = this._nextFocusable(e, n), this._focus(i), e.preventDefault())
            },
            _focusableElements: function() {
                var t = this.element.find(r).filter(function(t, n) {
                    return n.tabIndex >= 0 && e(n).is(":visible") && !e(n).is("[disabled]")
                });
                return this.element.is("[tabindex]") && t.push(this.element[0]), t
            },
            _sortFocusableElements: function(e) {
                var t, n;
                return i ? t = e.sort(function(e, t) {
                    return e.tabIndex - t.tabIndex
                }) : (n = "__k_index", e.each(function(e, t) {
                    t.setAttribute(n, e)
                }), t = e.sort(function(e, t) {
                    return e.tabIndex === t.tabIndex ? parseInt(e.getAttribute(n), 10) - parseInt(t.getAttribute(n), 10) : e.tabIndex - t.tabIndex
                }), e.removeAttr(n)), t
            },
            _nextFocusable: function(e, t) {
                var n = t.length,
                    i = t.index(e.target);
                return t.get((i + (e.shiftKey ? -1 : 1)) % n)
            },
            _focus: function(e) {
                return "IFRAME" == e.nodeName ? (e.contentWindow.document.body.focus(), t) : (e.focus(), "INPUT" == e.nodeName && e.setSelectionRange && this._haveSelectionRange(e) && e.setSelectionRange(0, e.value.length), t)
            },
            _haveSelectionRange: function(e) {
                var t = e.type.toLowerCase();
                return "text" === t || "search" === t || "url" === t || "tel" === t || "password" === t
            }
        }), l.Popup.TabKeyTrap = s
    }(window.kendo.jQuery), window.kendo
}, "function" == typeof define && define.amd ? define : function(e, t, n) {
    (n || t)()
}),
function(e, define) {
    define("kendo.list.min", ["kendo.data.min", "kendo.popup.min"], e)
}(function() {
    return function(e, t) {
        function n(e, n) {
            return e !== t && "" !== e && null !== e && ("boolean" === n ? e = !!e : "number" === n ? e = +e : "string" === n && (e = "" + e)), e
        }

        function i(e) {
            return e[e.length - 1]
        }

        function o(e) {
            var t = e.selectedIndex;
            return t > -1 ? e.options[t] : {}
        }

        function r(e, t) {
            var n, i, o, r, s = t.length,
                a = e.length,
                l = [],
                c = [];
            if (a)
                for (o = 0; o < a; o++) {
                    for (n = e[o], i = !1, r = 0; r < s; r++)
                        if (n === t[r]) {
                            i = !0, l.push({
                                index: o,
                                item: n
                            });
                            break
                        }
                    i || c.push(n)
                }
            return {
                changed: l,
                unchanged: c
            }
        }

        function s(t) {
            return !(!t || e.isEmptyObject(t)) && !(t.filters && !t.filters.length)
        }

        function a(t, n) {
            var i, o = !1;
            return t.filters && (i = e.grep(t.filters, function(e) {
                return o = a(e, n), e.filters ? e.filters.length : e.field != n
            }), o || t.filters.length === i.length || (o = !0), t.filters = i), o
        }
        var l, c, d = window.kendo,
            u = d.ui,
            h = d._outerHeight,
            p = /^\d+(\.\d+)?%$/i,
            f = u.Widget,
            m = d.keys,
            g = d.support,
            v = d.htmlEncode,
            _ = d._activeElement,
            b = d._outerWidth,
            w = d.data.ObservableArray,
            y = "id",
            k = "change",
            x = "k-state-focused",
            C = "k-state-hover",
            S = "k-i-loading",
            T = ".k-group-header",
            D = "_label",
            A = "open",
            E = "close",
            I = "cascade",
            M = "select",
            R = "selected",
            F = "requestStart",
            P = "requestEnd",
            z = e.extend,
            B = e.proxy,
            L = e.isArray,
            H = g.browser,
            N = "k-hidden",
            O = "width",
            V = H.msie,
            W = V && H.version < 9,
            U = /"/g,
            q = {
                ComboBox: "DropDownList",
                DropDownList: "ComboBox"
            },
            j = d.ui.DataBoundWidget.extend({
                init: function(t, n) {
                    var i, o = this,
                        r = o.ns;
                    f.fn.init.call(o, t, n), t = o.element, n = o.options, o._isSelect = t.is(M), o._isSelect && o.element[0].length && (n.dataSource || (n.dataTextField = n.dataTextField || "text", n.dataValueField = n.dataValueField || "value")), o.ul = e('<ul unselectable="on" class="k-list k-reset"/>').attr({
                        tabIndex: -1,
                        "aria-hidden": !0
                    }), o.list = e("<div class='k-list-container'/>").append(o.ul).on("mousedown" + r, B(o._listMousedown, o)), i = t.attr(y), i && (o.list.attr(y, i + "-list"), o.ul.attr(y, i + "_listbox")), n.columns && n.columns.length && (o.ul.removeClass("k-list").addClass("k-grid-list"), o._columnsHeader()), o._header(), o._noData(), o._footer(), o._accessors(), o._initValue()
                },
                options: {
                    valuePrimitive: !1,
                    footerTemplate: "",
                    headerTemplate: "",
                    noDataTemplate: "No data found."
                },
                setOptions: function(e) {
                    f.fn.setOptions.call(this, e), e && e.enable !== t && (e.enabled = e.enable), e.columns && e.columns.length && this._columnsHeader(), this._header(), this._noData(), this._footer(), this._renderFooter(), this._renderNoData()
                },
                focus: function() {
                    this._focused.focus()
                },
                readonly: function(e) {
                    this._editable({
                        readonly: e === t || e,
                        disable: !1
                    })
                },
                enable: function(e) {
                    this._editable({
                        readonly: !1,
                        disable: !(e = e === t || e)
                    })
                },
                _header: function() {
                    var n, i = this,
                        o = e(i.header),
                        r = i.options.headerTemplate;
                    return this._angularElement(o, "cleanup"), d.destroy(o), o.remove(), r ? (n = "function" != typeof r ? d.template(r) : r, o = e(n({})), i.header = o[0] ? o : null, i.list.prepend(o), this._angularElement(i.header, "compile"), t) : (i.header = null, t)
                },
                _columnsHeader: function() {
                    var t, n, i, o, r, s, a, l, c, u, h, f = this,
                        m = e(f.columnsHeader);
                    for (this._angularElement(m, "cleanup"), d.destroy(m), m.remove(), t = "<div class='k-grid-header'><div class='k-grid-header-wrap'><table>", n = "<colgroup>", i = "<tr>", o = 0; o < this.options.columns.length; o++) r = this.options.columns[o], s = r.title || r.field || "", a = r.headerTemplate || s, l = "function" != typeof a ? d.template(a) : a, c = r.width, u = parseInt(c, 10), h = "", c && !isNaN(u) && (h += "style='width:", h += u, h += p.test(c) ? "%" : "px", h += ";'"), n += "<col " + h + "/>", i += "<th class='k-header'>", i += l(r), i += "</th>";
                    n += "</colgroup>", i += "</tr>", t += n, t += i, t += "</table></div></div>", f.columnsHeader = m = e(t), f.list.prepend(m), this._angularElement(f.columnsHeader, "compile")
                },
                _noData: function() {
                    var n = this,
                        i = e(n.noData),
                        o = n.options.noDataTemplate;
                    return n.angular("cleanup", function() {
                        return {
                            elements: i
                        }
                    }), d.destroy(i), i.remove(), o ? (n.noData = e('<div class="k-nodata" style="display:none"><div></div></div>').appendTo(n.list), n.noDataTemplate = "function" != typeof o ? d.template(o) : o, t) : (n.noData = null, t)
                },
                _footer: function() {
                    var n = this,
                        i = e(n.footer),
                        o = n.options.footerTemplate;
                    return this._angularElement(i, "cleanup"), d.destroy(i), i.remove(), o ? (n.footer = e('<div class="k-footer"></div>').appendTo(n.list), n.footerTemplate = "function" != typeof o ? d.template(o) : o, t) : (n.footer = null, t)
                },
                _listOptions: function(t) {
                    var n = this,
                        i = n.options,
                        o = i.virtual,
                        r = {
                            change: B(n._listChange, n)
                        },
                        s = B(n._listBound, n);
                    return o = "object" == typeof o ? o : {}, t = e.extend({
                        autoBind: !1,
                        selectable: !0,
                        dataSource: n.dataSource,
                        click: B(n._click, n),
                        activate: B(n._activateItem, n),
                        columns: i.columns,
                        deactivate: B(n._deactivateItem, n),
                        dataBinding: function() {
                            n.trigger("dataBinding")
                        },
                        dataBound: s,
                        height: i.height,
                        dataValueField: i.dataValueField,
                        dataTextField: i.dataTextField,
                        groupTemplate: i.groupTemplate,
                        fixedGroupTemplate: i.fixedGroupTemplate,
                        template: i.template
                    }, t, o, r), t.template || (t.template = "#:" + d.expr(t.dataTextField, "data") + "#"), i.$angular && (t.$angular = i.$angular), t
                },
                _initList: function() {
                    var e = this,
                        t = e._listOptions({
                            selectedItemChange: B(e._listChange, e)
                        });
                    e.listView = e.options.virtual ? new d.ui.VirtualList(e.ul, t) : new d.ui.StaticList(e.ul, t), e.listView.bind("listBound", B(e._listBound, e)), e._setListValue()
                },
                _setListValue: function(e) {
                    e = e || this.options.value, e !== t && this.listView.value(e).done(B(this._updateSelectionState, this))
                },
                _updateSelectionState: e.noop,
                _listMousedown: function(e) {
                    this.filterInput && this.filterInput[0] === e.target || e.preventDefault()
                },
                _isFilterEnabled: function() {
                    var e = this.options.filter;
                    return e && "none" !== e
                },
                _hideClear: function() {
                    var e = this;
                    e._clear && e._clear.addClass(N)
                },
                _showClear: function() {
                    this._clear && this._clear.removeClass(N)
                },
                _clearValue: function() {
                    this._clearText(), this._accessor(""), this.listView.value([]), this._isSelect && (this._customOption = t), this._isFilterEnabled() && !this.options.enforceMinLength && (this._filter({
                        word: "",
                        open: !1
                    }), this.options.highlightFirst && this.listView.focus(0)), this._change()
                },
                _clearText: function() {
                    this.text("")
                },
                _clearFilter: function() {
                    this.options.virtual || this.listView.bound(!1), this._filterSource()
                },
                _filterSource: function(e, t) {
                    var n, i, o = this,
                        r = o.options,
                        l = r.filterFields && e && e.logic && e.filters && e.filters.length,
                        c = o.dataSource,
                        d = z({}, c.filter() || {}),
                        u = e || d.filters && d.filters.length && !e,
                        h = a(d, r.dataTextField);
                    if (this._clearFilterExpressions(d), !e && !h || !o.trigger("filtering", {
                            filter: e
                        })) return n = {
                        filters: [],
                        logic: "and"
                    }, l ? n.filters.push(e) : this._pushFilterExpression(n, e), s(d) && (n.logic === d.logic ? n.filters = n.filters.concat(d.filters) : n.filters.push(d)), o._cascading && this.listView.setDSFilter(n), i = z({}, {
                        page: u ? 1 : c.page(),
                        pageSize: u ? c.options.pageSize : c.pageSize(),
                        sort: c.sort(),
                        filter: c.filter(),
                        group: c.group(),
                        aggregate: c.aggregate()
                    }, {
                        filter: n
                    }), c[t ? "read" : "query"](c._mergeState(i))
                },
                _pushFilterExpression: function(t, n) {
                    s(n) && e.trim(n.value).length && t.filters.push(n)
                },
                _clearFilterExpressions: function(e) {
                    var t, n;
                    if (e.filters) {
                        for (n = 0; n < e.filters.length; n++) "fromFilter" in e.filters[n] && (t = n);
                        isNaN(t) || e.filters.splice(t, 1)
                    }
                },
                _angularElement: function(e, t) {
                    e && this.angular(t, function() {
                        return {
                            elements: e
                        }
                    })
                },
                _renderNoData: function() {
                    var e = this,
                        t = e.noData;
                    t && (this._angularElement(t, "cleanup"), t.children(":first").html(e.noDataTemplate({
                        instance: e
                    })), this._angularElement(t, "compile"))
                },
                _toggleNoData: function(t) {
                    e(this.noData).toggle(t)
                },
                _toggleHeader: function(e) {
                    var t = this.listView.content.prev(T);
                    t.toggle(e)
                },
                _renderFooter: function() {
                    var e = this,
                        t = e.footer;
                    t && (this._angularElement(t, "cleanup"), t.html(e.footerTemplate({
                        instance: e
                    })), this._angularElement(t, "compile"))
                },
                _allowOpening: function() {
                    return this.options.noDataTemplate || this.dataSource.flatView().length
                },
                _initValue: function() {
                    var e = this,
                        t = e.options.value;
                    null !== t ? e.element.val(t) : (t = e._accessor(), e.options.value = t), e._old = t
                },
                _ignoreCase: function() {
                    var e, t = this,
                        n = t.dataSource.reader.model;
                    n && n.fields && (e = n.fields[t.options.dataTextField], e && e.type && "string" !== e.type && (t.options.ignoreCase = !1))
                },
                _focus: function(e) {
                    return this.listView.focus(e)
                },
                _filter: function(e) {
                    var t, n, i = this,
                        o = i.options,
                        r = e.word,
                        s = o.filterFields,
                        a = o.dataTextField;
                    if (s && s.length)
                        for (t = {
                                logic: "or",
                                filters: [],
                                fromFilter: !0
                            }, n = 0; n < s.length; n++) this._pushFilterExpression(t, i._buildExpression(r, s[n]));
                    else t = i._buildExpression(r, a);
                    i._open = e.open, i._filterSource(t)
                },
                _buildExpression: function(e, t) {
                    var n = this,
                        i = n.options,
                        o = i.ignoreCase;
                    return {
                        value: o ? e.toLowerCase() : e,
                        field: t,
                        operator: i.filter,
                        ignoreCase: o
                    }
                },
                _clearButton: function() {
                    var t = this,
                        n = t.options.messages && t.options.messages.clear ? t.options.messages.clear : "clear";
                    t._clear || (t._clear = e('<span unselectable="on" class="k-icon k-clear-value k-i-close" title="' + n + '"></span>').attr({
                        role: "button",
                        tabIndex: -1
                    })), t.options.clearButton || t._clear.remove(), this._hideClear()
                },
                search: function(t) {
                    var n = this.options;
                    t = "string" == typeof t ? t : this._inputValue(), clearTimeout(this._typingTimeout), (!n.enforceMinLength && !t.length || t.length >= n.minLength) && (this._state = "filter", this.listView && (this.listView._emptySearch = !e.trim(t).length), this._isFilterEnabled() ? this._filter({
                        word: t,
                        open: !0
                    }) : this._searchByWord(t))
                },
                current: function(e) {
                    return this._focus(e)
                },
                items: function() {
                    return this.ul[0].children
                },
                destroy: function() {
                    var e = this,
                        t = e.ns;
                    f.fn.destroy.call(e), e._unbindDataSource(), e.listView.destroy(), e.list.off(t), e.popup.destroy(), e._form && e._form.off("reset", e._resetHandler)
                },
                dataItem: function(n) {
                    var i = this;
                    if (n === t) return i.listView.selectedDataItems()[0];
                    if ("number" != typeof n) {
                        if (i.options.virtual) return i.dataSource.getByUid(e(n).data("uid"));
                        n = e(i.items()).index(n)
                    }
                    return i.dataSource.flatView()[n]
                },
                _activateItem: function() {
                    var e = this.listView.focus();
                    e && this._focused.add(this.filterInput).attr("aria-activedescendant", e.attr("id"))
                },
                _deactivateItem: function() {
                    this._focused.add(this.filterInput).removeAttr("aria-activedescendant")
                },
                _accessors: function() {
                    var e = this,
                        t = e.element,
                        n = e.options,
                        i = d.getter,
                        o = t.attr(d.attr("text-field")),
                        r = t.attr(d.attr("value-field"));
                    !n.dataTextField && o && (n.dataTextField = o), !n.dataValueField && r && (n.dataValueField = r), e._text = i(n.dataTextField), e._value = i(n.dataValueField)
                },
                _aria: function(e) {
                    var n = this,
                        i = n.options,
                        o = n._focused.add(n.filterInput);
                    i.suggest !== t && o.attr("aria-autocomplete", i.suggest ? "both" : "list"), e = e ? e + " " + n.ul[0].id : n.ul[0].id, o.attr("aria-owns", e), n.ul.attr("aria-live", n._isFilterEnabled() ? "polite" : "off"), n._ariaLabel()
                },
                _ariaLabel: function() {
                    var t, n = this,
                        i = n._focused,
                        o = n.element,
                        r = o.attr("id"),
                        s = e('label[for="' + r + '"]'),
                        a = o.attr("aria-label"),
                        l = o.attr("aria-labelledby");
                    i !== o && (a ? i.attr("aria-label", a) : l ? i.attr("aria-labelledby", l) : s.length && (t = s.attr("id") || n._generateLabelId(s, r), i.attr("aria-labelledby", t)))
                },
                _generateLabelId: function(e, t) {
                    var n = t + D;
                    return e.attr("id", n), n
                },
                _blur: function() {
                    var e = this;
                    e._change(), e.close()
                },
                _change: function() {
                    var e, i = this,
                        o = i.selectedIndex,
                        r = i.options.value,
                        s = i.value();
                    i._isSelect && !i.listView.bound() && r && (s = r), s !== n(i._old, typeof s) ? e = !0 : i._valueBeforeCascade !== t && i._valueBeforeCascade !== n(i._old, typeof i._valueBeforeCascade) && i._userTriggered ? e = !0 : o === t || o === i._oldIndex || i.listView.isFiltered() || (e = !0), e && (i._valueBeforeCascade = i._old = null === i._old || "" === s ? s : i.dataItem() ? i.options.dataValueField ? i.dataItem()[i.options.dataValueField] : i.dataItem() : null, i._oldIndex = o, i._typing || i.element.trigger(k), i.trigger(k)), i.typing = !1
                },
                _data: function() {
                    return this.dataSource.view()
                },
                _enable: function() {
                    var e = this,
                        n = e.options,
                        i = e.element.is("[disabled]");
                    n.enable !== t && (n.enabled = n.enable), !n.enabled || i ? e.enable(!1) : e.readonly(e.element.is("[readonly]"))
                },
                _dataValue: function(e) {
                    var n = this._value(e);
                    return n === t && (n = this._text(e)), n
                },
                _offsetHeight: function() {
                    var t = 0,
                        n = this.listView.content.prevAll(":visible");
                    return n.each(function() {
                        var n = e(this);
                        t += h(n, !0)
                    }), t
                },
                _height: function(n) {
                    var i, o, r, s = this,
                        a = s.list,
                        l = s.options.height,
                        c = s.popup.visible();
                    if (n || s.options.noDataTemplate) {
                        if (o = a.add(a.parent(".k-animation-container")).show(), !a.is(":visible")) return o.hide(), t;
                        l = s.listView.content[0].scrollHeight > l ? l : "auto", o.height(l), "auto" !== l && (i = s._offsetHeight(), r = h(e(s.footer)) || 0, l = l - i - r), s.listView.content.height(l), c || o.hide()
                    }
                    return l
                },
                _openHandler: function(e) {
                    this._adjustListWidth(), this.trigger(A) ? e.preventDefault() : (this._focused.attr("aria-expanded", !0), this.ul.attr("aria-hidden", !1))
                },
                _adjustListWidth: function() {
                    var e, t, n = this,
                        i = n.list,
                        o = i[0].style.width,
                        r = n.wrapper;
                    if (i.data(O) || !o) return e = window.getComputedStyle ? window.getComputedStyle(r[0], null) : 0, t = parseFloat(e && e.width) || b(r), e && H.msie && (t += parseFloat(e.paddingLeft) + parseFloat(e.paddingRight) + parseFloat(e.borderLeftWidth) + parseFloat(e.borderRightWidth)), o = "border-box" !== i.css("box-sizing") ? t - (b(i) - i.width()) : t, i.css({
                        fontFamily: r.css("font-family"),
                        width: n.options.autoWidth ? "auto" : o,
                        minWidth: o,
                        whiteSpace: n.options.autoWidth ? "nowrap" : "normal"
                    }).data(O, o), !0
                },
                _closeHandler: function(e) {
                    this.trigger(E) ? e.preventDefault() : (this._focused.attr("aria-expanded", !1), this.ul.attr("aria-hidden", !0))
                },
                _focusItem: function() {
                    var e = this.listView,
                        n = !e.focus(),
                        o = i(e.select());
                    o === t && this.options.highlightFirst && n && (o = 0), o !== t ? e.focus(o) : n && e.scrollToIndex(0)
                },
                _calculateGroupPadding: function(e) {
                    var t = this.ul.children(".k-first:first"),
                        n = this.listView.content.prev(T),
                        i = 0;
                    n[0] && "none" !== n[0].style.display && ("auto" !== e && (i = d.support.scrollbar()), i += parseFloat(t.css("border-right-width"), 10) + parseFloat(t.children(".k-group").css("padding-right"), 10), n.css("padding-right", i))
                },
                _calculatePopupHeight: function(e) {
                    var t = this._height(this.dataSource.flatView().length || e);
                    this._calculateGroupPadding(t), this._calculateColumnsHeaderPadding(t)
                },
                _calculateColumnsHeaderPadding: function(e) {
                    var t, n, i;
                    this.options.columns && this.options.columns.length && (t = this, n = g.isRtl(t.wrapper), i = d.support.scrollbar(), t.columnsHeader.css(n ? "padding-left" : "padding-right", "auto" !== e ? i : 0))
                },
                _resizePopup: function(e) {
                    this.options.virtual || (this.popup.element.is(":visible") ? this._calculatePopupHeight(e) : this.popup.one("open", function(e) {
                        return B(function() {
                            this._calculatePopupHeight(e)
                        }, this)
                    }.call(this, e)))
                },
                _popup: function() {
                    var e = this;
                    e.popup = new u.Popup(e.list, z({}, e.options.popup, {
                        anchor: e.wrapper,
                        open: B(e._openHandler, e),
                        close: B(e._closeHandler, e),
                        animation: e.options.animation,
                        isRtl: g.isRtl(e.wrapper),
                        autosize: e.options.autoWidth
                    }))
                },
                _makeUnselectable: function() {
                    W && this.list.find("*").not(".k-textbox").attr("unselectable", "on")
                },
                _toggleHover: function(t) {
                    e(t.currentTarget).toggleClass(C, "mouseenter" === t.type)
                },
                _toggle: function(e, n) {
                    var i = this,
                        o = g.mobileOS && (g.touch || g.MSPointers || g.pointers);
                    e = e !== t ? e : !i.popup.visible(), n || o || i._focused[0] === _() || (i._prevent = !0, i._focused.focus(), i._prevent = !1), i[e ? A : E]()
                },
                _triggerCascade: function() {
                    var e = this;
                    e._cascadeTriggered && e.value() === n(e._cascadedValue, typeof e.value()) || (e._cascadedValue = e.value(), e._cascadeTriggered = !0, e.trigger(I, {
                        userTriggered: e._userTriggered
                    }))
                },
                _triggerChange: function() {
                    this._valueBeforeCascade !== this.value() && this.trigger(k)
                },
                _unbindDataSource: function() {
                    var e = this;
                    e.dataSource.unbind(F, e._requestStartHandler).unbind(P, e._requestEndHandler).unbind("error", e._errorHandler)
                },
                requireValueMapper: function(e, t) {
                    var n = (e.value instanceof Array ? e.value.length : e.value) || (t instanceof Array ? t.length : t);
                    if (n && e.virtual && "function" != typeof e.virtual.valueMapper) throw Error("ValueMapper is not provided while the value is being set. See http://docs.telerik.com/kendo-ui/controls/editors/combobox/virtualization#the-valuemapper-function")
                }
            });
        z(j, {
            inArray: function(e, t) {
                var n, i, o = t.children;
                if (!e || e.parentNode !== t) return -1;
                for (n = 0, i = o.length; n < i; n++)
                    if (e === o[n]) return n;
                return -1
            },
            unifyType: n
        }), d.ui.List = j, u.Select = j.extend({
            init: function(e, t) {
                j.fn.init.call(this, e, t), this._initial = this.element.val()
            },
            setDataSource: function(e) {
                var t, n = this;
                n.options.dataSource = e, n._dataSource(), n.listView.bound() && (n._initialIndex = null, n.listView._current = null), n.listView.setDataSource(n.dataSource), n.options.autoBind && n.dataSource.fetch(), t = n._parentWidget(), t && n._cascadeSelect(t)
            },
            close: function() {
                this.popup.close()
            },
            select: function(e) {
                var n = this;
                return e === t ? n.selectedIndex : n._select(e).done(function() {
                    n._cascadeValue = n._old = n._accessor(), n._oldIndex = n.selectedIndex
                })
            },
            _accessor: function(e, t) {
                return this[this._isSelect ? "_accessorSelect" : "_accessorInput"](e, t)
            },
            _accessorInput: function(e) {
                var n = this.element[0];
                return e === t ? n.value : (null === e && (e = ""), n.value = e, t)
            },
            _accessorSelect: function(e, n) {
                var i, r = this.element[0];
                return e === t ? o(r).value || "" : (o(r).selected = !1, n === t && (n = -1), i = null !== e && "" !== e, i && n == -1 ? this._custom(e) : e ? r.value = e : r.selectedIndex = n, t)
            },
            _syncValueAndText: function() {
                return !0
            },
            _custom: function(t) {
                var n = this,
                    i = n.element,
                    o = n._customOption;
                o || (o = e("<option/>"), n._customOption = o, i.append(o)), o.text(t), o[0].selected = !0
            },
            _hideBusy: function() {
                var e = this;
                clearTimeout(e._busy), e._arrowIcon.removeClass(S), e._focused.attr("aria-busy", !1), e._busy = null, e._showClear()
            },
            _showBusy: function(e) {
                var t = this;
                e.isDefaultPrevented() || (t._request = !0, t._busy || (t._busy = setTimeout(function() {
                    t._arrowIcon && (t._focused.attr("aria-busy", !0), t._arrowIcon.addClass(S), t._hideClear())
                }, 100)))
            },
            _requestEnd: function() {
                this._request = !1, this._hideBusy()
            },
            _dataSource: function() {
                var t, n = this,
                    i = n.element,
                    o = n.options,
                    r = o.dataSource || {};
                r = e.isArray(r) ? {
                    data: r
                } : r, n._isSelect && (t = i[0].selectedIndex, t > -1 && (o.index = t), r.select = i, r.fields = [{
                    field: o.dataTextField
                }, {
                    field: o.dataValueField
                }]), n.dataSource ? n._unbindDataSource() : (n._requestStartHandler = B(n._showBusy, n), n._requestEndHandler = B(n._requestEnd, n), n._errorHandler = B(n._hideBusy, n)), n.dataSource = d.data.DataSource.create(r).bind(F, n._requestStartHandler).bind(P, n._requestEndHandler).bind("error", n._errorHandler)
            },
            _firstItem: function() {
                this.listView.focusFirst()
            },
            _lastItem: function() {
                this.listView.focusLast()
            },
            _nextItem: function() {
                this.listView.focusNext()
            },
            _prevItem: function() {
                this.listView.focusPrev()
            },
            _move: function(e) {
                var n, i, o, r, s, a, l = this,
                    c = l.listView,
                    d = e.keyCode,
                    u = d === m.DOWN;
                if (d === m.UP || u) {
                    if (e.altKey) l.toggle(u);
                    else {
                        if (!c.bound() && !l.ul[0].firstChild) return l._fetch || (l.dataSource.one(k, function() {
                            l._fetch = !1, l._move(e)
                        }), l._fetch = !0, l._filterSource()), e.preventDefault(), !0;
                        if (o = l._focus(), l._fetch || o && !o.hasClass("k-state-selected") || (u ? (l._nextItem(), l._focus() || l._lastItem()) : (l._prevItem(), l._focus() || l._firstItem())), n = c.dataItemByIndex(c.getElementIndex(l._focus())), l.trigger(M, {
                                dataItem: n,
                                item: l._focus()
                            })) return l._focus(o), t;
                        l._select(l._focus(), !0).done(function() {
                            l.popup.visible() || l._blur(), l._cascadedValue = null === l._cascadedValue ? l.value() : l.dataItem() ? l.dataItem()[l.options.dataValueField] || l.dataItem() : null
                        })
                    }
                    e.preventDefault(), i = !0
                } else if (d === m.ENTER || d === m.TAB) {
                    if (l.popup.visible() && e.preventDefault(), o = l._focus(), n = l.dataItem(), l.popup.visible() || n && l.text() === l._text(n) || (o = null), r = l.filterInput && l.filterInput[0] === _(), o) {
                        if (n = c.dataItemByIndex(c.getElementIndex(o)), s = !0, n && (s = l._value(n) !== j.unifyType(l.value(), typeof l._value(n))), s && l.trigger(M, {
                                dataItem: n,
                                item: o
                            })) return;
                        l._select(o)
                    } else l.input && ((l._syncValueAndText() || l._isSelect) && l._accessor(l.input.val()), l.listView.value(l.input.val()));
                    l._focusElement && l._focusElement(l.wrapper), r && d === m.TAB ? l.wrapper.focusout() : l._blur(), l.close(), i = !0
                } else d === m.ESC ? (l.popup.visible() && e.preventDefault(), l.close(), i = !0) : !l.popup.visible() || d !== m.PAGEDOWN && d !== m.PAGEUP || (e.preventDefault(), a = d === m.PAGEDOWN ? 1 : -1, c.scrollWith(a * c.screenHeight()), i = !0);
                return i
            },
            _fetchData: function() {
                var e = this,
                    t = !!e.dataSource.view().length;
                e._request || e.options.cascadeFrom || e.listView.bound() || e._fetch || t || (e._fetch = !0, e.dataSource.fetch().done(function() {
                    e._fetch = !1
                }))
            },
            _options: function(e, n, i) {
                var r, s, a, l, c = this,
                    d = c.element,
                    u = d[0],
                    h = e.length,
                    p = "",
                    f = 0;
                for (n && (p = n); f < h; f++) r = "<option", s = e[f], a = c._text(s), l = c._value(s), l !== t && (l += "", l.indexOf('"') !== -1 && (l = l.replace(U, "&quot;")), r += ' value="' + l + '"'), r += ">", a !== t && (r += v(a)), r += "</option>", p += r;
                d.html(p), i !== t && (u.value = i, u.value && !i && (u.selectedIndex = -1)), u.selectedIndex !== -1 && (r = o(u), r && r.setAttribute(R, R))
            },
            _reset: function() {
                var t = this,
                    n = t.element,
                    i = n.attr("form"),
                    o = i ? e("#" + i) : n.closest("form");
                o[0] && (t._resetHandler = function() {
                    setTimeout(function() {
                        t.value(t._initial)
                    })
                }, t._form = o.on("reset", t._resetHandler))
            },
            _parentWidget: function() {
                var t, n, i = this.options.name;
                if (this.options.cascadeFrom) return t = e("#" + this.options.cascadeFrom), n = t.data("kendo" + i), n || (n = t.data("kendo" + q[i])), n
            },
            _cascade: function() {
                var e, t = this,
                    n = t.options,
                    i = n.cascadeFrom;
                if (i) {
                    if (e = t._parentWidget(), !e) return;
                    t._cascadeHandlerProxy = B(t._cascadeHandler, t), t._cascadeFilterRequests = [], n.autoBind = !1, e.bind("set", function() {
                        t.one("set", function(e) {
                            t._selectedValue = e.value || t._accessor()
                        })
                    }), e.first(I, t._cascadeHandlerProxy), e.listView.bound() ? (t._toggleCascadeOnFocus(), t._cascadeSelect(e)) : (e.one("dataBound", function() {
                        t._toggleCascadeOnFocus(), e.popup.visible() && e._focused.focus()
                    }), e.value() || t.enable(!1))
                }
            },
            _toggleCascadeOnFocus: function() {
                var e = this,
                    t = e._parentWidget(),
                    n = V ? "blur" : "focusout";
                t._focused.add(t.filterInput).bind("focus", function() {
                    t.unbind(I, e._cascadeHandlerProxy), t.first(k, e._cascadeHandlerProxy)
                }), t._focused.add(t.filterInput).bind(n, function() {
                    t.unbind(k, e._cascadeHandlerProxy), t.first(I, e._cascadeHandlerProxy)
                })
            },
            _cascadeHandler: function(e) {
                var t = this._parentWidget(),
                    n = this.value();
                this._userTriggered = e.userTriggered, this.listView.bound() && this._clearSelection(t, !0), this._cascadeSelect(t, n)
            },
            _cascadeChange: function(e) {
                var t = this,
                    n = t._accessor() || t._selectedValue;
                t._cascadeFilterRequests.length || (t._selectedValue = null), t._userTriggered ? t._clearSelection(e, !0) : n ? (n !== t.listView.value()[0] && t.value(n), t.dataSource.view()[0] && t.selectedIndex !== -1 || t._clearSelection(e, !0)) : t.dataSource.flatView().length && t.select(t.options.index), t.enable(), t._triggerCascade(), t._triggerChange(), t._userTriggered = !1
            },
            _cascadeSelect: function(e, n) {
                var i, o, r = this,
                    s = e.dataItem(),
                    l = s ? e._value(s) : null,
                    c = r.options.cascadeFromField || e.options.dataValueField;
                r._valueBeforeCascade = n !== t ? n : r.value(), l || 0 === l ? (i = r.dataSource.filter() || {}, a(i, c), o = function() {
                    var t = r._cascadeFilterRequests.shift();
                    t && r.unbind("dataBound", t), t = r._cascadeFilterRequests[0], t && r.first("dataBound", t), r._cascadeChange(e)
                }, r._cascadeFilterRequests.push(o), 1 === r._cascadeFilterRequests.length && r.first("dataBound", o), r._cascading = !0, r._filterSource({
                    field: c,
                    operator: "eq",
                    value: l
                }), r._cascading = !1) : (r.enable(!1), r._clearSelection(e), r._triggerCascade(), r._triggerChange(), r._userTriggered = !1)
            }
        }), l = ".StaticList", c = d.ui.DataBoundWidget.extend({
            init: function(t, n) {
                f.fn.init.call(this, t, n), this.element.attr("role", "listbox").on("click" + l, "li", B(this._click, this)).on("mouseenter" + l, "li", function() {
                    e(this).addClass(C)
                }).on("mouseleave" + l, "li", function() {
                    e(this).removeClass(C)
                }), g.touch && this._touchHandlers(), "multiple" === this.options.selectable && this.element.attr("aria-multiselectable", !0), this.content = this.element.wrap("<div class='k-list-scroller' unselectable='on'></div>").parent(), this.header = this.content.before('<div class="k-group-header" style="display:none"></div>').prev(), this.bound(!1), this._optionID = d.guid(), this._selectedIndices = [], this._view = [], this._dataItems = [], this._values = [];
                var i = this.options.value;
                i && (this._values = e.isArray(i) ? i.slice(0) : [i]), this._getter(), this._templates(), this.setDataSource(this.options.dataSource), this._onScroll = B(function() {
                    var e = this;
                    clearTimeout(e._scrollId), e._scrollId = setTimeout(function() {
                        e._renderHeader()
                    }, 50)
                }, this)
            },
            options: {
                name: "StaticList",
                dataValueField: null,
                valuePrimitive: !1,
                selectable: !0,
                template: null,
                groupTemplate: null,
                fixedGroupTemplate: null
            },
            events: ["click", k, "activate", "deactivate", "dataBinding", "dataBound", "selectedItemChange"],
            setDataSource: function(t) {
                var n, i = this,
                    o = t || {};
                o = e.isArray(o) ? {
                    data: o
                } : o, o = d.data.DataSource.create(o), i.dataSource ? (i.dataSource.unbind(k, i._refreshHandler), n = i.value(), i.value([]), i.bound(!1), i.value(n)) : i._refreshHandler = B(i.refresh, i), i.setDSFilter(o.filter()), i.dataSource = o.bind(k, i._refreshHandler), i._fixedHeader()
            },
            _touchHandlers: function() {
                var t, n, i = this,
                    o = function(e) {
                        return (e.originalEvent || e).changedTouches[0].pageY
                    };
                i.element.on("touchstart" + l, function(e) {
                    t = o(e)
                }), i.element.on("touchend" + l, function(r) {
                    r.isDefaultPrevented() || (n = o(r), Math.abs(n - t) < 10 && (r.preventDefault(), i.trigger("click", {
                        item: e(r.target)
                    })))
                })
            },
            skip: function() {
                return this.dataSource.skip()
            },
            setOptions: function(e) {
                f.fn.setOptions.call(this, e), this._getter(), this._templates(), this._render()
            },
            destroy: function() {
                this.element.off(l), this._refreshHandler && this.dataSource.unbind(k, this._refreshHandler), clearTimeout(this._scrollId), f.fn.destroy.call(this)
            },
            dataItemByIndex: function(e) {
                return this.dataSource.flatView()[e]
            },
            screenHeight: function() {
                return this.content[0].clientHeight
            },
            scrollToIndex: function(e) {
                var t = this.element[0].children[e];
                t && this.scroll(t)
            },
            scrollWith: function(e) {
                this.content.scrollTop(this.content.scrollTop() + e)
            },
            scroll: function(e) {
                if (e) {
                    e[0] && (e = e[0]);
                    var t = this.content[0],
                        n = e.offsetTop,
                        i = e.offsetHeight,
                        o = t.scrollTop,
                        r = t.clientHeight,
                        s = n + i;
                    o > n ? o = n : s > o + r && (o = s - r), t.scrollTop = o
                }
            },
            selectedDataItems: function(e) {
                return e === t ? this._dataItems.slice() : (this._dataItems = e, this._values = this._getValues(e), t)
            },
            _getValues: function(t) {
                var n = this._valueGetter;
                return e.map(t, function(e) {
                    return n(e)
                })
            },
            focusNext: function() {
                var e = this.focus();
                e = e ? e.next() : 0, this.focus(e)
            },
            focusPrev: function() {
                var e = this.focus();
                e = e ? e.prev() : this.element[0].children.length - 1, this.focus(e)
            },
            focusFirst: function() {
                this.focus(this.element[0].children[0])
            },
            focusLast: function() {
                this.focus(i(this.element[0].children))
            },
            focus: function(n) {
                var o, r = this,
                    s = r._optionID;
                return n === t ? r._current : (n = i(r._get(n)), n = e(this.element[0].children[n]), r._current && (r._current.removeClass(x).removeAttr(y), r.trigger("deactivate")), o = !!n[0], o && (n.addClass(x), r.scroll(n), n.attr("id", s)), r._current = o ? n : null, r.trigger("activate"), t)
            },
            focusIndex: function() {
                return this.focus() ? this.focus().index() : t
            },
            skipUpdate: function(e) {
                this._skipUpdate = e
            },
            select: function(n) {
                var o, r, s, a = this,
                    l = a.options.selectable,
                    c = "multiple" !== l && l !== !1,
                    d = a._selectedIndices,
                    u = [],
                    h = [];
                return n === t ? d.slice() : (n = a._get(n), 1 === n.length && n[0] === -1 && (n = []), r = e.Deferred().resolve(), s = a.isFiltered(), s && !c && a._deselectFiltered(n) ? r : c && !s && e.inArray(i(n), d) !== -1 ? (a._dataItems.length && a._view.length && (a._dataItems = [a._view[d[0]].item]), r) : (o = a._deselect(n), h = o.removed, n = o.indices, n.length && (c && (n = [i(n)]), u = a._select(n)), (u.length || h.length) && (a._valueComparer = null, a.trigger(k, {
                    added: u,
                    removed: h
                })), r))
            },
            removeAt: function(e) {
                return this._selectedIndices.splice(e, 1), this._values.splice(e, 1), this._valueComparer = null, {
                    position: e,
                    dataItem: this._dataItems.splice(e, 1)[0]
                }
            },
            setValue: function(t) {
                t = e.isArray(t) || t instanceof w ? t.slice(0) : [t], this._values = t, this._valueComparer = null
            },
            value: function(n) {
                var i, o = this,
                    r = o._valueDeferred;
                return n === t ? o._values.slice() : (o.setValue(n), r && "resolved" !== r.state() || (o._valueDeferred = r = e.Deferred()), o.bound() && (i = o._valueIndices(o._values), "multiple" === o.options.selectable && o.select(-1), o.select(i), r.resolve()), o._skipUpdate = !1, r)
            },
            items: function() {
                return this.element.children(".k-item")
            },
            _click: function(t) {
                t.isDefaultPrevented() || this.trigger("click", {
                    item: e(t.currentTarget)
                }) || this.select(t.currentTarget)
            },
            _valueExpr: function(e, t) {
                var i, o, r = this,
                    s = 0,
                    a = [];
                if (!r._valueComparer || r._valueType !== e) {
                    for (r._valueType = e; s < t.length; s++) a.push(n(t[s], e));
                    i = "for (var idx = 0; idx < " + a.length + "; idx++) { if (current === values[idx]) {   return idx; }} return -1;", o = Function("current", "values", i), r._valueComparer = function(e) {
                        return o(e, a)
                    }
                }
                return r._valueComparer
            },
            _dataItemPosition: function(e, t) {
                var n = this._valueGetter(e),
                    i = this._valueExpr(typeof n, t);
                return i(n)
            },
            _getter: function() {
                this._valueGetter = d.getter(this.options.dataValueField)
            },
            _deselect: function(t) {
                var n, i, o, r = this,
                    s = r.element[0].children,
                    a = r.options.selectable,
                    l = r._selectedIndices,
                    c = r._dataItems,
                    d = r._values,
                    u = [],
                    h = 0,
                    p = 0;
                if (t = t.slice(), a !== !0 && t.length) {
                    if ("multiple" === a)
                        for (; h < t.length; h++)
                            if (i = t[h], e(s[i]).hasClass("k-state-selected"))
                                for (n = 0; n < l.length; n++)
                                    if (o = l[n], o === i) {
                                        e(s[o]).removeClass("k-state-selected").attr("aria-selected", !1), u.push({
                                            position: n + p,
                                            dataItem: c.splice(n, 1)[0]
                                        }), l.splice(n, 1), t.splice(h, 1), d.splice(n, 1), p += 1, h -= 1, n -= 1;
                                        break
                                    }
                } else {
                    for (; h < l.length; h++) e(s[l[h]]).removeClass("k-state-selected").attr("aria-selected", !1), u.push({
                        position: h,
                        dataItem: c[h]
                    });
                    r._values = [], r._dataItems = [], r._selectedIndices = []
                }
                return {
                    indices: t,
                    removed: u
                }
            },
            _deselectFiltered: function(t) {
                for (var n, i, o, r = this.element[0].children, s = [], a = 0; a < t.length; a++) i = t[a], n = this._view[i].item, o = this._dataItemPosition(n, this._values), o > -1 && (s.push(this.removeAt(o)), e(r[i]).removeClass("k-state-selected"));
                return !!s.length && (this.trigger(k, {
                    added: [],
                    removed: s
                }), !0)
            },
            _select: function(t) {
                var n, o, r = this,
                    s = r.element[0].children,
                    a = r._view,
                    l = [],
                    c = 0;
                for (i(t) !== -1 && r.focus(t); c < t.length; c++) o = t[c], n = a[o], o !== -1 && n && (n = n.item, r._selectedIndices.push(o), r._dataItems.push(n), r._values.push(r._valueGetter(n)), e(s[o]).addClass("k-state-selected").attr("aria-selected", !0), l.push({
                    dataItem: n
                }));
                return l
            },
            getElementIndex: function(t) {
                return e(t).data("offset-index")
            },
            _get: function(e) {
                return "number" == typeof e ? e = [e] : L(e) || (e = this.getElementIndex(e), e = [e !== t ? e : -1]), e
            },
            _template: function() {
                var e = this,
                    t = e.options,
                    n = t.template;
                return n ? (n = d.template(n), n = function(e) {
                    return '<li tabindex="-1" role="option" unselectable="on" class="k-item">' + n(e) + "</li>"
                }) : n = d.template('<li tabindex="-1" role="option" unselectable="on" class="k-item">${' + d.expr(t.dataTextField, "data") + "}</li>", {
                    useWithBlock: !1
                }), n
            },
            _templates: function() {
                var e, t, n, i, o, r = this.options,
                    s = {
                        template: r.template,
                        groupTemplate: r.groupTemplate,
                        fixedGroupTemplate: r.fixedGroupTemplate
                    };
                if (r.columns)
                    for (t = 0; t < r.columns.length; t++) n = r.columns[t], i = n.field ? "" + n.field : "text", s["column" + t] = n.template || "#: " + i + "#";
                for (o in s) e = s[o], e && "function" != typeof e && (s[o] = d.template(e));
                this.templates = s
            },
            _normalizeIndices: function(e) {
                for (var n = [], i = 0; i < e.length; i++) e[i] !== t && n.push(e[i]);
                return n
            },
            _valueIndices: function(e, t) {
                var n, i = this._view,
                    o = 0;
                if (t = t ? t.slice() : [], !e.length) return [];
                for (; o < i.length; o++) n = this._dataItemPosition(i[o].item, e), n !== -1 && (t[n] = o);
                return this._normalizeIndices(t)
            },
            _firstVisibleItem: function() {
                for (var t = this.element[0], n = this.content[0], i = n.scrollTop, o = e(t.children[0]).height(), r = Math.floor(i / o) || 0, s = t.children[r] || t.lastChild, a = s.offsetTop < i; s;)
                    if (a) {
                        if (s.offsetTop + o > i || !s.nextSibling) break;
                        s = s.nextSibling
                    } else {
                        if (s.offsetTop <= i || !s.previousSibling) break;
                        s = s.previousSibling
                    }
                return this._view[e(s).data("offset-index")]
            },
            _fixedHeader: function() {
                this.isGrouped() && this.templates.fixedGroupTemplate ? (this.header.show(), this.content.scroll(this._onScroll)) : (this.header.hide(), this.content.off("scroll", this._onScroll))
            },
            _renderHeader: function() {
                var e, t = this.templates.fixedGroupTemplate;
                t && (e = this._firstVisibleItem(), e && e.group && this.header.html(t(e.group)))
            },
            _renderItem: function(e) {
                var t = '<li tabindex="-1" role="option" unselectable="on" class="k-item',
                    n = e.item,
                    i = 0 !== e.index,
                    o = e.selected,
                    r = this.isGrouped(),
                    s = this.options.columns && this.options.columns.length;
                return i && e.newGroup && (t += " k-first"), e.isLastGroupedItem && s && (t += " k-last"), o && (t += " k-state-selected"), t += '" aria-selected="' + (o ? "true" : "false") + '" data-offset-index="' + e.index + '">', t += s ? this._renderColumns(n) : this.templates.template(n), i && e.newGroup ? t += s ? '<div class="k-cell k-group-cell"><span>' + this.templates.groupTemplate(e.group) + "</span></div>" : '<div class="k-group">' + this.templates.groupTemplate(e.group) + "</div>" : r && s && (t += "<div class='k-cell k-spacer-cell'></div>"), t + "</li>"
            },
            _renderColumns: function(e) {
                var t, n, i, o, r = "";
                for (t = 0; t < this.options.columns.length; t++) n = this.options.columns[t].width, i = parseInt(n, 10), o = "", n && !isNaN(i) && (o += "style='width:", o += i, o += p.test(n) ? "%" : "px", o += ";'"), r += "<span class='k-cell' " + o + ">", r += this.templates["column" + t](e), r += "</span>";
                return r
            },
            _render: function() {
                var e, t, n, i, o = "",
                    r = 0,
                    s = 0,
                    a = [],
                    l = this.dataSource.view(),
                    c = this.value(),
                    d = this.isGrouped();
                if (d)
                    for (r = 0; r < l.length; r++)
                        for (t = l[r], n = !0, i = 0; i < t.items.length; i++) e = {
                            selected: this._selected(t.items[i], c),
                            item: t.items[i],
                            group: t.value,
                            newGroup: n,
                            isLastGroupedItem: i === t.items.length - 1,
                            index: s
                        }, a[s] = e, s += 1, o += this._renderItem(e), n = !1;
                else
                    for (r = 0; r < l.length; r++) e = {
                        selected: this._selected(l[r], c),
                        item: l[r],
                        index: r
                    }, a[r] = e, o += this._renderItem(e);
                this._view = a, this.element[0].innerHTML = o, d && a.length && this._renderHeader()
            },
            _selected: function(e, t) {
                var n = !this.isFiltered() || "multiple" === this.options.selectable;
                return n && this._dataItemPosition(e, t) !== -1
            },
            setDSFilter: function(e) {
                this._lastDSFilter = z({}, e)
            },
            isFiltered: function() {
                return this._lastDSFilter || this.setDSFilter(this.dataSource.filter()), !d.data.Query.compareFilters(this.dataSource.filter(), this._lastDSFilter)
            },
            refresh: function(e) {
                var t, n = this,
                    i = e && e.action,
                    o = n.options.skipUpdateOnBind,
                    s = "itemchange" === i;
                n.trigger("dataBinding"), n._angularItems("cleanup"), n._fixedHeader(), n._render(), n.bound(!0), s || "remove" === i ? (t = r(n._dataItems, e.items), t.changed.length && (s ? n.trigger("selectedItemChange", {
                    items: t.changed
                }) : n.value(n._getValues(t.unchanged)))) : n.isFiltered() || n._skipUpdate || n._emptySearch ? (n.focus(0), n._skipUpdate && (n._skipUpdate = !1, n._selectedIndices = n._valueIndices(n._values, n._selectedIndices))) : o || i && "add" !== i || n.value(n._values), n._valueDeferred && n._valueDeferred.resolve(), n._angularItems("compile"), n.trigger("dataBound")
            },
            bound: function(e) {
                return e === t ? this._bound : (this._bound = e, t)
            },
            isGrouped: function() {
                return (this.dataSource.group() || []).length
            }
        }), u.plugin(c)
    }(window.kendo.jQuery), window.kendo
}, "function" == typeof define && define.amd ? define : function(e, t, n) {
    (n || t)()
}),
function(e, define) {
    define("kendo.treeview.min", ["kendo.data.min", "kendo.treeview.draganddrop.min"], e)
}(function() {
    return function(e, t) {
        function n(e) {
            return function(t) {
                var n = t.children(".k-animation-container");
                return n.length || (n = t), n.children(e)
            }
        }

        function i(e) {
            return p.template(e, {
                useWithBlock: !1
            })
        }

        function o(e) {
            return e.find("> div .k-checkbox-wrapper [type=checkbox]")
        }

        function r(e) {
            return function(t, n) {
                n = n.closest(q);
                var i, o = n.parent();
                return o.parent().is("li") && (i = o.parent()), this._dataSourceMove(t, o, i, function(t, i) {
                    var o = this.dataItem(n),
                        r = o ? o.parent().indexOf(o) : n.index();
                    return this._insert(t.data(), i, r + e)
                })
            }
        }

        function s(t, n) {
            for (var i; t && "ul" != t.nodeName.toLowerCase();) i = t, t = t.nextSibling, 3 == i.nodeType && (i.nodeValue = e.trim(i.nodeValue)), h.test(i.className) ? n.insertBefore(i, n.firstChild) : n.appendChild(i)
        }

        function a(t) {
            var n = t.children("div"),
                i = t.children("ul"),
                o = n.children(".material-icons"),
                r = t.children(":checkbox"),
                a = n.children(".k-in");
            t.hasClass("k-treeview") || (n.length || (n = e("<div />").prependTo(t)), !o.length && i.length ? o = e("<span class='k-icon' />").prependTo(n) : i.length && i.children().length || (o.remove(), i.remove()), r.length && e("<span class='k-checkbox-wrapper' />").appendTo(n).append(r), a.length || (a = t.children("a").eq(0).addClass("k-in k-link"), a.length || (a = e("<span class='k-in' />")), a.appendTo(n), n.length && s(n[0].nextSibling, a[0])))
        }
        var l, c, d, u, h, p = window.kendo,
            f = p.ui,
            m = p.data,
            g = e.extend,
            v = p.template,
            _ = e.isArray,
            b = f.Widget,
            w = m.HierarchicalDataSource,
            y = e.proxy,
            k = p.keys,
            x = ".kendoTreeView",
            C = ".kendoTreeViewTemp",
            S = "select",
            T = "check",
            D = "navigate",
            A = "expand",
            E = "change",
            I = "error",
            M = "checked",
            R = "indeterminate",
            F = "collapse",
            P = "dragstart",
            z = "drag",
            B = "drop",
            L = "dragend",
            H = "dataBound",
            N = "click",
            O = "undefined",
            V = "k-state-hover",
            W = "k-treeview",
            U = ":visible",
            q = ".k-item",
            j = "string",
            G = "aria-label",
            $ = "aria-selected",
            K = "aria-disabled",
            Y = "k-state-disabled",
            Q = {
                text: "dataTextField",
                url: "dataUrlField",
                spriteCssClass: "dataSpriteCssClassField",
                imageUrl: "dataImageUrlField"
            },
            X = function (e) {
                return e instanceof p.jQuery || e instanceof window.jQuery
            },
            Z = function (e) {
                return "object" == typeof HTMLElement ? e instanceof HTMLElement : e && "object" == typeof e && 1 === e.nodeType && typeof e.nodeName === j
            },
            link = null;
          
        c = n(".k-group"), d = n(".k-group,.k-content"), u = function(e) {
            return e.children("div").children(".k-icon")
        }, h = /k-sprite/, l = p.ui.DataBoundWidget.extend({
            init: function(e, t) {
                var n, i = this,
                    o = !1,
                    r = t && !!t.dataSource;
                link = (t && t.link ? t.link : link);
                _(t) && (t = {
                    dataSource: t
                }), t && typeof t.loadOnDemand == O && _(t.dataSource) && (t.loadOnDemand = !1), b.prototype.init.call(i, e, t), e = i.element, t = i.options, n = e.is("ul") && e || e.hasClass(W) && e.children("ul"), o = !r && n.length, o && (t.dataSource.list = n), i._animation(), i._accessors(), i._templates(), e.hasClass(W) ? (i.wrapper = e, i.root = e.children("ul").eq(0)) : (i._wrapper(), n && (i.root = e, i._group(i.wrapper))), i._tabindex(), i.root.attr("role", "tree"), i._dataSource(o), i._attachEvents(), i._dragging(), o ? i._syncHtmlAndDataSource() : t.autoBind && (i._progress(!0), i.dataSource.fetch()), t.checkboxes && t.checkboxes.checkChildren && i.updateIndeterminate(), i.element[0].id && (i._ariaId = p.format("{0}_tv_active", i.element[0].id)), p.notify(i)
            },
            _attachEvents: function() {
                var t = this,
                    n = ".k-in:not(.k-state-selected,.k-state-disabled)",
                    i = "mouseenter";
                t.wrapper.on(i + x, ".k-in.k-state-selected", function(e) {
                    e.preventDefault()
                }).on(i + x, n, function() {
                    e(this).addClass(V)
                }).on("mouseleave" + x, n, function() {
                    e(this).removeClass(V)
                }).on(N + x, n, y(t._click, t)).on("dblclick" + x, ".k-in:not(.k-state-disabled)", y(t._toggleButtonClick, t)).on(N + x, ".k-i-expand,.k-i-collapse", y(t._toggleButtonClick, t)).on("keydown" + x, y(t._keydown, t)).on("keypress" + x, y(t._keypress, t)).on("focus" + x, y(t._focus, t)).on("blur" + x, y(t._blur, t)).on("mousedown" + x, ".k-in,.k-checkbox-wrapper :checkbox,.k-i-expand,.k-i-collapse", y(t._mousedown, t)).on("change" + x, ".k-checkbox-wrapper :checkbox", y(t._checkboxChange, t)).on("click" + x, ".checkbox-span", y(t._checkboxLabelClick, t)).on("click" + x, ".k-request-retry", y(t._retryRequest, t)).on("click" + x, ".k-link.k-state-disabled", function(e) {
                    e.preventDefault()
                }).on("click" + x, function(n) {
                    e(n.target).is(":kendoFocusable") || t.focus()
                })
            },
            _checkboxLabelClick: function(t) {
                var n = e(t.target.previousSibling);
                n.is("[disabled]") || (n.prop("checked", !n.prop("checked")), n.trigger("change"))
            },
            _syncHtmlAndDataSource: function(e, t) {
                e = e || this.root, t = t || this.dataSource;
                var n, i, r, s, a, l = t.view(),
                    c = p.attr("uid"),
                    d = p.attr("expanded"),
                    u = this.options.checkboxes,
                    h = e.children("li");
                for (n = 0; n < h.length; n++) r = l[n], s = r.uid, i = h.eq(n), i.attr("role", "treeitem").attr(c, s).attr($, i.hasClass("k-state-selected")), r.expanded = "true" === i.attr(d), u && (a = o(i), r.checked = a.prop(M), a.attr("id", "_" + s), a.next(".k-checkbox-label").attr("for", "_" + s)), this._syncHtmlAndDataSource(i.children("ul"), r.children)
            },
            _animation: function() {
                var e = this.options,
                    t = e.animation,
                    n = t.collapse && "effects" in t.collapse,
                    i = g({}, t.expand, t.collapse);
                n || (i = g(i, {
                    reverse: !0
                })), t === !1 && (t = {
                    expand: {
                        effects: {}
                    },
                    collapse: {
                        hide: !0,
                        effects: {}
                    }
                }), t.collapse = g(i, {
                    hide: !0
                }), e.animation = t
            },
            _dragging: function() {
                var t, n = this.options.dragAndDrop,
                    i = this.dragging;
                n && !i ? (t = this, this.dragging = new f.HierarchicalDragAndDrop(this.element, {
                    reorderable: !0,
                    $angular: this.options.$angular,
                    autoScroll: this.options.autoScroll,
                    filter: "div:not(.k-state-disabled) .k-in",
                    allowedContainers: ".k-treeview",
                    itemSelector: ".k-treeview .k-item",
                    hintText: y(this._hintText, this),
                    contains: function(t, n) {
                        return e.contains(t, n)
                    },
                    dropHintContainer: function(e) {
                        return e
                    },
                    itemFromTarget: function(e) {
                        var t = e.closest(".k-top,.k-mid,.k-bot");
                        return {
                            item: t,
                            content: e.closest(".k-in"),
                            first: t.hasClass("k-top"),
                            last: t.hasClass("k-bot")
                        }
                    },
                    dropPositionFrom: function(e) {
                        return e.prevAll(".k-in").length > 0 ? "after" : "before"
                    },
                    dragstart: function(e) {
                        return t.trigger(P, {
                            sourceNode: e[0]
                        })
                    },
                    drag: function(e) {
                        t.trigger(z, {
                            originalEvent: e.originalEvent,
                            sourceNode: e.source[0],
                            dropTarget: e.target[0],
                            pageY: e.pageY,
                            pageX: e.pageX,
                            statusClass: e.status,
                            setStatusClass: e.setStatus
                        })
                    },
                    drop: function(n) {
                        var i = e(n.dropTarget),
                            o = i.closest("a");
                        return o && o.attr("href") && t._tempPreventNavigation(o), t.trigger(B, {
                            originalEvent: n.originalEvent,
                            sourceNode: n.source,
                            destinationNode: n.destination,
                            valid: n.valid,
                            setValid: function(e) {
                                this.valid = e, n.setValid(e)
                            },
                            dropTarget: n.dropTarget,
                            dropPosition: n.position
                        })
                    },
                    dragend: function(e) {
                        function n(n) {
                            t.options.checkboxes && t.options.checkboxes.checkChildren && t.updateIndeterminate(), t.trigger(L, {
                                originalEvent: e.originalEvent,
                                sourceNode: n && n[0],
                                destinationNode: o[0],
                                dropPosition: r
                            })
                        }
                        var i = e.source,
                            o = e.destination,
                            r = e.position;
                        "over" == r ? t.append(i, o, n) : ("before" == r ? i = t.insertBefore(i, o) : "after" == r && (i = t.insertAfter(i, o)), n(i))
                    }
                })) : !n && i && (i.destroy(), this.dragging = null)
            },
            _tempPreventNavigation: function(e) {
                e.on(N + x + C, function(t) {
                    t.preventDefault(), e.off(N + x + C)
                })
            },
            _hintText: function(e) {
                return this.templates.dragClue({
                    item: this.dataItem(e),
                    treeview: this.options
                })
            },
            _templates: function() {
                var e = this,
                    t = e.options,
                    n = y(e._fieldAccessor, e);
                t.template && typeof t.template == j ? t.template = v(t.template) : t.template || (t.template = i("# var text = " + n("text") + "(data.item); ## if (typeof data.item.encoded != 'undefined' && data.item.encoded === false) {##= text ## } else { ##: text ## } #")), e._checkboxes(), e.templates = {
                    wrapperCssClass: function(e, t) {
                        var n = "k-item",
                            i = t.index;
                        return e.firstLevel && 0 === i && (n += " k-first"), i == e.length - 1 && (n += " k-last"), n
                    },
                    cssClass: function(e, t) {
                        var n = "",
                            i = t.index,
                            o = e.length - 1;
                        return e.firstLevel && 0 === i && (n += "k-top "), n += 0 === i && i != o ? "k-top" : i == o ? "k-bot" : "k-mid"
                    },
                    textClass: function(e, t) {
                        var n = "k-in";
                        return t && (n += " k-link"), e.enabled === !1 && (n += " k-state-disabled"), e.selected === !0 && (n += " k-state-selected"), n
                    },
                    toggleButtonClass: function(e) {
                        var t = "material-icons k-icon";
                        return t += e.expanded !== !0 ? " k-i-expand" : " k-i-collapse"
                    },
                    groupAttributes: function(e) {
                        var t = "";
                        return e.firstLevel || (t = "role='group'"), t + (e.expanded !== !0 ? " style='display:none'" : "")
                    },
                    groupCssClass: function(e) {
                        var t = "k-group";
                        return e.firstLevel && (t += " k-treeview-lines"), t
                    },
                    dragClue: i("#= data.treeview.template(data) #"),
                    group: i("<ul class='#= data.r.groupCssClass(data.group) #'#= data.r.groupAttributes(data.group) #>#= data.renderItems(data) #</ul>"),
                    itemContent: i("# var imageUrl = " + n("imageUrl") + "(data.item); ## var spriteCssClass = " + n("spriteCssClass") + "(data.item); ## if (imageUrl) { #<img class='k-image' alt='' src='#= imageUrl #'># } ## if (spriteCssClass) { #<span class='k-sprite #= spriteCssClass #' /># } ##= data.treeview.template(data) #"),
                    itemElement: i("# var item = data.item, r = data.r; ## var url = " + n("url") + "(item); #<div class='#= r.cssClass(data.group, item) #'># if (item.hasChildren) { #<i class='#= r.toggleButtonClass(item) #'/># } ## if (data.treeview.checkboxes) { #<span class='k-checkbox-wrapper' role='presentation'>#= data.treeview.checkboxes.template(data) #</span># } ## var tag = url ? 'a' : 'span'; ## var textAttr = url ? ' href=\\'' + url + '\\'' : ''; #<#=tag# class='#= r.textClass(item, !!url) #'#= textAttr #>#= r.itemContent(data) #</#=tag#></div>"),
                    item: i("# var item = data.item, r = data.r; #<li role='treeitem' class='#= r.wrapperCssClass(data.group, item) #' " + p.attr("uid") + "='#= item.uid #' aria-selected='#= item.selected ? \"true\" : \"false\" #' #=item.enabled === false ? \"aria-disabled='true'\" : ''## if (item.expanded) { #data-expanded='true' aria-expanded='true'# } #>#= r.itemElement(data) #</li>"),
                    loading: i("<div class='k-icon k-i-loading' /> #: data.messages.loading #"),
                    retry: i("#: data.messages.requestFailed # <button class='k-button k-request-retry'>#: data.messages.retry #</button>")
                }
            },
            items: function() {
                return this.element.find(".k-item > div:first-child")
            },
            setDataSource: function(t) {
                var n = this.options;
                n.dataSource = t, this._dataSource(), n.checkboxes && n.checkboxes.checkChildren && this.dataSource.one("change", e.proxy(this.updateIndeterminate, this, null)), this.options.autoBind && this.dataSource.fetch()
            },
            _bindDataSource: function() {
                this._refreshHandler = y(this.refresh, this), this._errorHandler = y(this._error, this), this.dataSource.bind(E, this._refreshHandler), this.dataSource.bind(I, this._errorHandler)
            },
            _unbindDataSource: function() {
                var e = this.dataSource;
                e && (e.unbind(E, this._refreshHandler), e.unbind(I, this._errorHandler))
            },
            _dataSource: function(e) {
                function t(e) {
                    for (var n = 0; n < e.length; n++) e[n]._initChildren(), e[n].children.fetch(), t(e[n].children.view())
                }
                var n = this,
                    i = n.options,
                    o = i.dataSource;
                o = _(o) ? {
                    data: o
                } : o, n._unbindDataSource(), o.fields || (o.fields = [{
                    field: "text"
                }, {
                    field: "url"
                }, {
                    field: "spriteCssClass"
                }, {
                    field: "imageUrl"
                }]), n.dataSource = o = w.create(o), e && (o.fetch(), t(o.view())), n._bindDataSource()
            },
            events: [P, z, B, L, H, A, F, S, E, D, T],
            options: {
                name: "TreeView",
                dataSource: {},
                animation: {
                    expand: {
                        effects: "expand:vertical",
                        duration: 200
                    },
                    collapse: {
                        duration: 100
                    }
                },
                messages: {
                    loading: "Loading...",
                    requestFailed: "Request failed.",
                    retry: "Retry"
                },
                dragAndDrop: !1,
                checkboxes: !1,
                autoBind: !0,
                autoScroll: !1,
                loadOnDemand: !0,
                template: "",
                dataTextField: null
            },
            _accessors: function() {
                var e, t, n, i = this,
                    o = i.options,
                    r = i.element;
                for (e in Q) t = o[Q[e]], n = r.attr(p.attr(e + "-field")), !t && n && (t = n), t || (t = e), _(t) || (t = [t]), o[Q[e]] = t
            },
            _fieldAccessor: function(t) {
                var n = this.options[Q[t]],
                    i = n.length,
                    o = "(function(item) {";
                return 0 === i ? o += "return item['" + t + "'];" : (o += "var levels = [" + e.map(n, function(e) {
                    return "function(d){ return " + p.expr(e) + "}"
                }).join(",") + "];", o += "return levels[Math.min(item.level(), " + i + "-1)](item)"), o += "})"
            },
            setOptions: function(e) {
                b.fn.setOptions.call(this, e), this._animation(), this._dragging(), this._templates()
            },
            _trigger: function(e, t) {
                return this.trigger(e, {
                    node: t.closest(q)[0]
                })
            },
            _setChecked: function(t, n) {
                if (t && e.isFunction(t.view))
                    for (var i = 0, o = t.view(); i < o.length; i++) o[i].enabled !== !1 && o[i].set(M, n), o[i].children && this._setChecked(o[i].children, n)
            },
            _setIndeterminate: function(e) {
                var t, n, i, r = c(e),
                    s = !0;
                if (r.length && (t = o(r.children()), n = t.length)) {
                    if (n > 1) {
                        for (i = 1; i < n; i++)
                            if (t[i].checked != t[i - 1].checked || t[i].indeterminate || t[i - 1].indeterminate) {
                                s = !1;
                                break
                            }
                    } else s = !t[0].indeterminate;
                    return o(e).data(R, !s).prop(R, !s).prop(M, s && t[0].checked)
                }
            },
            updateIndeterminate: function(e) {
                var t, n, i, o;
                if (e = e || this.wrapper, t = c(e).children(), t.length) {
                    for (n = 0; n < t.length; n++) this.updateIndeterminate(t.eq(n));
                    i = this._setIndeterminate(e), o = this.dataItem(e), i && i.prop(M) ? o.checked = !0 : o && delete o.checked
                }
            },
            _bubbleIndeterminate: function(e, t) {
                if (e.length) {
                    t || this.updateIndeterminate(e);
                    var n, i = this.parent(e);
                    i.length && (this._setIndeterminate(i), n = i.children("div").find(".k-checkbox-wrapper :checkbox"), this._skip = !0, n.prop(R) === !1 ? this.dataItem(i).set(M, n.prop(M)) : this.dataItem(i).set(M, !1), this._skip = !1, this._bubbleIndeterminate(i, !0))
                }
            },
            _checkboxChange: function(t) {
                var n = e(t.target),
                    i = n.prop(M),
                    o = n.closest(q),
                    r = this.dataItem(o);
                this._preventChange || r.checked != i && (r.set(M, i), this._trigger(T, o))
            },
            _toggleButtonClick: function(t) {
                var n = e(t.currentTarget).closest(q);
                n.is("[aria-disabled='true']") || this.toggle(n)
            },
            _mousedown: function(t) {
                var n = this,
                    i = e(t.currentTarget),
                    o = e(t.currentTarget).closest(q),
                    r = p.support.browser;
                o.is("[aria-disabled='true']") || ((r.msie || r.edge) && i.is(":checkbox") && (i.prop(R) ? (n._preventChange = !1, i.prop(M, !i.prop(M)), i.trigger(E), i.on(N + x, function(e) {
                    e.preventDefault()
                }), n._preventChange = !0) : (i.off(N + x), n._preventChange = !1)), n._clickTarget = o, n.current(o))
            },
            _focusable: function(e) {
                return e && e.length && e.is(":visible") && !e.find(".k-in:first").hasClass(Y)
            },
            _focus: function() {
                var t = this.select(),
                    n = this._clickTarget;
                p.support.touch || (n && n.length && (t = n), this._focusable(t) || (t = this.current()), this._focusable(t) || (t = this._nextVisible(e())), this.current(t))
            },
            focus: function() {
                var e, t = this.wrapper,
                    n = t[0],
                    i = [],
                    o = [],
                    r = document.documentElement;
                do n = n.parentNode, n.scrollHeight > n.clientHeight && (i.push(n), o.push(n.scrollTop)); while (n != r);
                for (t.focus(), e = 0; e < i.length; e++) i[e].scrollTop = o[e]
            },
            _blur: function() {
                this.current().find(".k-in:first").removeClass("k-state-focused")
            },
            _enabled: function(e) {
                return !e.children("div").children(".k-in").hasClass(Y)
            },
            parent: function(t) {
                var n, i, o = /\bk-treeview\b/,
                    r = /\bk-item\b/;
                typeof t == j && (t = this.element.find(t)), Z(t) || (t = t[0]), i = r.test(t.className);
                do t = t.parentNode, r.test(t.className) && (i ? n = t : i = !0); while (!o.test(t.className) && !n);
                return e(n)
            },
            _nextVisible: function(e) {
                function t(e) {
                    for (; e.length && !e.next().length;) e = i.parent(e);
                    return e.next().length ? e.next() : e
                }
                var n, i = this,
                    o = i._expanded(e);
                return e.length && e.is(":visible") ? o ? (n = c(e).children().first(), n.length || (n = t(e))) : n = t(e) : n = i.root.children().eq(0), n
            },
            _previousVisible: function(e) {
                var t, n, i = this;
                if (!e.length || e.prev().length)
                    for (n = e.length ? e.prev() : i.root.children().last(); i._expanded(n) && (t = c(n).children().last(), t.length);) n = t;
                else n = i.parent(e) || e;
                return n
            },
            _keydown: function(n) {
                var i, o = this,
                    r = n.keyCode,
                    s = o.current(),
                    a = o._expanded(s),
                    l = s.find(".k-checkbox-wrapper:first :checkbox"),
                    c = p.support.isRtl(o.element);
                n.target == n.currentTarget && (!c && r == k.RIGHT || c && r == k.LEFT ? a ? i = o._nextVisible(s) : s.find(".k-in:first").hasClass(Y) || o.expand(s) : !c && r == k.LEFT || c && r == k.RIGHT ? a && !s.find(".k-in:first").hasClass(Y) ? o.collapse(s) : (i = o.parent(s), o._enabled(i) || (i = t)) : r == k.DOWN ? i = o._nextVisible(s) : r == k.UP ? i = o._previousVisible(s) : r == k.HOME ? i = o._nextVisible(e()) : r == k.END ? i = o._previousVisible(e()) : r != k.ENTER || s.find(".k-in:first").hasClass(Y) ? r == k.SPACEBAR && l.length && (s.find(".k-in:first").hasClass(Y) || (l.prop(M, !l.prop(M)).data(R, !1).prop(R, !1), o._checkboxChange({
                    target: l
                })), i = s) : s.find(".k-in:first").hasClass("k-state-selected") || o._trigger(S, s) || o.select(s), i && (n.preventDefault(), s[0] != i[0] && (o._trigger(D, i), o.current(i))))
            },
            _keypress: function(e) {
                var t, n = this,
                    i = 300,
                    o = n._getSelectedNode();
                e.keyCode !== k.ENTER && e.keyCode !== k.SPACEBAR && (n._match || (n._match = ""), n._match += String.fromCharCode(e.keyCode), clearTimeout(n._matchTimer), n._matchTimer = setTimeout(function() {
                    n._match = ""
                }, i), t = o && n._matchNextByText(Array.prototype.indexOf.call(this.element.find(".k-item"), o[0]), n._match), t || (t = n._matchNextByText(-1, n._match)), n.select(t))
            },
            _matchNextByText: function(t, n) {
                return e(this.element).find(".k-in").filter(function(i, o) {
                    return i > t && e(o).is(":visible") && !e(o).hasClass(Y) && 0 === e(o).text().toLowerCase().indexOf(n)
                }).closest(q)[0]
            },
            _click: function(t) {
                var n, i = this,
                    o = e(t.currentTarget),
                    r = d(o.closest(q)),
                    s = o.attr("href");
                n = s ? "#" == s || s.indexOf("#" + this.element.id + "-") >= 0 : r.length && !r.children().length, n && t.preventDefault(), o.hasClass(".k-state-selected") || i._trigger(S, o) || i.select(o)
            },
            _wrapper: function() {
                var e, t, n = this,
                    i = n.element,
                    o = "k-widget k-treeview";
                i.is("ul") ? (e = i.wrap("<div />").parent(), t = i) : (e = i, t = e.children("ul").eq(0)), n.wrapper = e.addClass(o), n.root = t
            },
            _getSelectedNode: function() {
                return this.element.find(".k-state-selected").closest(q)
            },
            _group: function(e) {
                var t = this,
                    n = e.hasClass(W),
                    i = {
                        firstLevel: n,
                        expanded: n || t._expanded(e)
                    },
                    o = e.children("ul");
                o.addClass(t.templates.groupCssClass(i)).css("display", i.expanded ? "" : "none"), t._nodes(o, i)
            },
            _nodes: function(t, n) {
                var i, o = this,
                    r = t.children("li");
                n = g({
                    length: r.length
                }, n), r.each(function(t, r) {
                    r = e(r), i = {
                        index: t,
                        expanded: o._expanded(r)
                    }, a(r), o._updateNodeClasses(r, n, i), o._group(r)
                })
            },
            _checkboxes: function() {
                var e, t = this.options,
                    n = t.checkboxes;
                n && (e = "<input type='checkbox' tabindex='-1' #= (item.enabled === false) ? 'disabled' : '' # #= item.checked ? 'checked' : '' #", n.name && (e += " name='" + n.name + "'"), e += " id='_#= item.uid #' class='k-checkbox' /><span class='k-checkbox-label checkbox-span'></span>", n = g({
                    template: e
                }, t.checkboxes), typeof n.template == j && (n.template = v(n.template)), t.checkboxes = n)
            },
            _updateNodeClasses: function(e, t, n) {
                var i, o, r = e.children("div"),
                    s = e.children("ul"),
                    a = this.templates;
                e.hasClass("k-treeview") || (n = n || {}, n.expanded = typeof n.expanded != O ? n.expanded : this._expanded(e), n.index = typeof n.index != O ? n.index : e.index(), n.enabled = typeof n.enabled != O ? n.enabled : !r.children(".k-in").hasClass("k-state-disabled"), t = t || {}, t.firstLevel = typeof t.firstLevel != O ? t.firstLevel : e.parent().parent().hasClass(W), t.length = typeof t.length != O ? t.length : e.parent().children().length, e.removeClass("k-first k-last").addClass(a.wrapperCssClass(t, n)), r.removeClass("k-top k-mid k-bot").addClass(a.cssClass(t, n)), i = r.children(".k-in"), o = i[0] && "a" == i[0].nodeName.toLowerCase(), i.removeClass("k-in k-link k-state-default k-state-disabled").addClass(a.textClass(n, o)), (s.length || "true" == e.attr("data-hasChildren")) && (r.children(".k-icon").removeClass("k-i-expand k-i-collapse").addClass(a.toggleButtonClass(n)), s.addClass("k-group")), this._checkboxAria(e))
            },
            _processNodes: function(t, n) {
                var i = this;
                i.element.find(t).each(function(t, o) {
                    n.call(i, t, e(o).closest(q))
                })
            },
            dataItem: function(t) {
                var n = e(t).closest(q).attr(p.attr("uid")),
                    i = this.dataSource;
                return i && i.getByUid(n)
            },
            _insertNode: function(t, n, i, o, r) {
                var s, l, d, u, h = this,
                    p = c(i),
                    f = p.children().length + 1,
                    m = {
                        firstLevel: i.hasClass(W),
                        expanded: !r,
                        length: f
                    },
                    g = "",
                    v = function(e, t) {
                        e.appendTo(t)
                    };
                for (d = 0; d < t.length; d++) u = t[d], u.index = n + d, g += h._renderItem({
                    group: m,
                    item: u
                });
                if (l = e(g), l.length) {
                    for (h.angular("compile", function() {
                            return {
                                elements: l.get(),
                                data: t.map(function(e) {
                                    return {
                                        dataItem: e
                                    }
                                })
                            }
                        }), p.length || (p = e(h._renderGroup({
                            group: m
                        })).appendTo(i)), o(l, p), i.hasClass("k-item") && (a(i), h._updateNodeClasses(i)), h._updateNodeClasses(l.prev().first()), h._updateNodeClasses(l.next().last()), h._checkboxAria(l), d = 0; d < t.length; d++) u = t[d], u.hasChildren && (s = u.children.data(), s.length && h._insertNode(s, u.index, l.eq(d), v, !h._expanded(l.eq(d))));
                    return l
                }
            },
            _checkboxAria: function(t) {
                var n;
                t.each(function(t, i) {
                    i = e(i), n = i.find(".k-in:first").text(), e(i).find("> div .k-checkbox-wrapper [type=checkbox]").attr(G, n)
                })
            },
            _updateNodes: function(t, n) {
                function i(e, t) {
                    e.find(".k-checkbox-wrapper :checkbox").not("[disabled]").prop(M, t).data(R, !1).prop(R, !1)
                }
                var o, r, s, a, l, c, u, h = this,
                    p = {
                        treeview: h.options,
                        item: a
                    },
                    m = "expanded" != n && "checked" != n;
                if ("selected" == n) a = t[0], r = h.findByUid(a.uid).find(".k-in:first").removeClass("k-state-hover").toggleClass("k-state-selected", a[n]).end(), a[n] && h.current(r), r.attr($, !!a[n]);
                else {
                    for (u = e.map(t, function(e) {
                            return h.findByUid(e.uid).children("div")
                        }), m && h.angular("cleanup", function() {
                            return {
                                elements: u
                            }
                        }), o = 0; o < t.length; o++) p.item = a = t[o], s = u[o], r = s.parent(), m && s.children(".k-in").html(h.templates.itemContent(p)), n == M ? (l = a[n], i(s, l), h.options.checkboxes.checkChildren && (i(r.children(".k-group"), l), h._setChecked(a.children, l), h._bubbleIndeterminate(r))) : "expanded" == n ? h._toggle(r, a, a[n]) : "enabled" == n && (r.find(".k-checkbox-wrapper :checkbox").prop("disabled", !a[n]), c = !d(r).is(U), r.removeAttr(K), a[n] || (a.selected && a.set("selected", !1), a.expanded && a.set("expanded", !1), c = !0, r.attr($, !1).attr(K, !0)), h._updateNodeClasses(r, {}, {
                        enabled: a[n],
                        expanded: !c
                    })), s.length && this.trigger("itemChange", {
                        item: s,
                        data: a,
                        ns: f
                    });
                    m && h.angular("compile", function() {
                        return {
                            elements: u,
                            data: e.map(t, function(e) {
                                return [{
                                    dataItem: e
                                }]
                            })
                        }
                    })
                }
            },
            _appendItems: function(e, t, n) {
                var i, o, r, s = c(n),
                    a = s.children(),
                    l = !this._expanded(n);
                this.element === n ? (i = this.dataSource.data(), o = this.dataSource.view(), r = o.length < i.length ? o : i, e = r.indexOf(t[0])) : t.length && (e = t[0].parent().indexOf(t[0])), typeof e == O && (e = a.length), this._insertNode(t, e, n, function(t, n) {
                    e >= a.length ? t.appendTo(n) : t.insertBefore(a.eq(e))
                }, l), this._expanded(n) && (this._updateNodeClasses(n), c(n).css("display", "block"))
            },
            _refreshChildren: function(e, t, n) {
                var i, o, r, s = this.options,
                    l = s.loadOnDemand,
                    d = s.checkboxes && s.checkboxes.checkChildren;
                if (c(e).empty(), t.length)
                    for (this._appendItems(n, t, e), o = c(e).children(), l && d && this._bubbleIndeterminate(o.last()), i = 0; i < o.length; i++) r = o.eq(i), this.trigger("itemChange", {
                        item: r.children("div"),
                        data: this.dataItem(r),
                        ns: f
                    });
                else a(e)
            },
            _refreshRoot: function(t) {
                var n, i, o, r = this._renderGroup({
                    items: t,
                    group: {
                        firstLevel: !0,
                        expanded: !0
                    }
                });
                for (this.root.length ? (this._angularItems("cleanup"), n = e(r), this.root.attr("class", n.attr("class")).html(n.html())) : this.root = this.wrapper.html(r).children("ul"), this.root.attr("role", "tree"), i = this.root.children(".k-item"), o = 0; o < t.length; o++) this.trigger("itemChange", {
                    item: i.eq(o),
                    data: t[o],
                    ns: f
                });
                this._angularItems("compile")
            },
            refresh: function(e) {
                var n, i, o = e.node,
                    r = e.action,
                    s = e.items,
                    a = this.wrapper,
                    l = this.options,
                    c = l.loadOnDemand,
                    d = l.checkboxes && l.checkboxes.checkChildren;
                if (!this._skip) {
                    if (e.field) {
                        if (!s[0] || !s[0].level) return;
                        return this._updateNodes(s, e.field)
                    }
                    if (o && (a = this.findByUid(o.uid), this._progress(a, !1)), d && "remove" != r) {
                        for (i = !1, n = 0; n < s.length; n++)
                            if ("checked" in s[n]) {
                                i = !0;
                                break
                            }
                        if (!i && o && o.checked)
                            for (n = 0; n < s.length; n++) s[n].checked = !0
                    }
                    if ("add" == r ? this._appendItems(e.index, s, a) : "remove" == r ? this._remove(this.findByUid(s[0].uid), !1) : "itemchange" == r ? this._updateNodes(s) : "itemloaded" == r ? this._refreshChildren(a, s, e.index) : this._refreshRoot(s), "remove" != r)
                        for (n = 0; n < s.length; n++)(!c || s[n].expanded || s[n]._loaded) && s[n].load();
                    this.trigger(H, {
                        node: o ? a : t
                    }), this.options.checkboxes.checkChildren && this.updateIndeterminate()
                }
            },
            _error: function(e) {
                var t = e.node && this.findByUid(e.node.uid),
                    n = this.templates.retry({
                        messages: this.options.messages
                    });
                t ? (this._progress(t, !1), this._expanded(t, !1), u(t).addClass("k-i-reload"), e.node.loaded(!1)) : (this._progress(!1), this.element.html(n))
            },
            _retryRequest: function(e) {
                e.preventDefault(), this.dataSource.fetch()
            },
            expand: function(e) {
                this._processNodes(e, function(e, t) {
                    this.toggle(t, !0)
                })
            },
            collapse: function(e) {
                this._processNodes(e, function(e, t) {
                    this.toggle(t, !1)
                })
            },
            enable: function(e, t) {
                "boolean" == typeof e ? (t = e, e = this.items()) : t = 2 != arguments.length || !!t, this._processNodes(e, function(e, n) {
                    this.dataItem(n).set("enabled", t)
                })
            },
            current: function(n) {
                var i = this,
                    o = i._current,
                    r = i.element,
                    s = i._ariaId;
                return arguments.length > 0 && n && n.length ? (o && (o[0].id === s && o.removeAttr("id"), o.find(".k-in:first").removeClass("k-state-focused")), o = i._current = e(n, r).closest(q), o.find(".k-in:first").addClass("k-state-focused"), s = o[0].id || s, s && (i.wrapper.removeAttr("aria-activedescendant"), o.attr("id", s), i.wrapper.attr("aria-activedescendant", s)), t) : (o || (o = i._nextVisible(e())), o)
            },
            select: function (n) { 
                var i = this,
                    o = i.element;
                var v = (arguments.length ? (n = e(n, o).closest(q), o.find(".k-state-selected").each(function () {
                    var t = i.dataItem(this);
                    t ? (t.set("selected", !1), delete t.selected) : e(this).removeClass("k-state-selected")
                }), n.length && (i.dataItem(n).set("selected", !0), i._clickTarget = n), i.trigger(E), t) : o.find(".k-state-selected").closest(q));
                
                var dataItem = i.dataItem(v);
                if (dataItem && dataItem.value && link){
                    console.log(dataItem.value);
                    link = link.replace("{val}", dataItem.value);
                    window.location = link;
                }
                return v;
            },
            _toggle: function(e, t, n) {
                var i, o = this.options,
                    r = d(e),
                    s = n ? "expand" : "collapse";
                r.data("animating") || (i = t && t.loaded(), n && !i ? (o.loadOnDemand && this._progress(e, !0), r.remove(), t.load()) : (this._updateNodeClasses(e, {}, {
                    expanded: n
                }), n || r.css("height", r.height()).css("height"), r.kendoStop(!0, !0).kendoAnimate(g({
                    reset: !0
                }, o.animation[s], {
                    complete: function() {
                        n && r.css("height", "")
                    }
                }))))
            },
            toggle: function(t, n) {
                t = e(t), u(t).is(".k-i-expand, .k-i-collapse") && (1 == arguments.length && (n = !this._expanded(t)), this._expanded(t, n))
            },
            destroy: function() {
                var e = this;
                b.fn.destroy.call(e), e.wrapper.off(x), e.wrapper.find(".k-checkbox-wrapper :checkbox").off(x), e._unbindDataSource(), e.dragging && e.dragging.destroy(), p.destroy(e.element), e.root = e.wrapper = e.element = null
            },
            _expanded: function(e, n, i) {
                var o = p.attr("expanded"),
                    r = this.dataItem(e),
                    s = n,
                    a = s ? "expand" : "collapse";
                return 1 == arguments.length ? "true" === e.attr(o) || r && r.expanded : (d(e).data("animating") || !i && this._trigger(a, e) || (r && (r.set("expanded", s), s = r.expanded), s ? (e.attr(o, "true"), e.attr("aria-expanded", "true")) : (e.removeAttr(o), e.attr("aria-expanded", "false"))), t)
            },
            _progress: function(e, t) {
                var n = this.element,
                    i = this.templates.loading({
                        messages: this.options.messages
                    });
                1 == arguments.length ? (t = e, t ? n.html(i) : n.empty()) : u(e).toggleClass("k-i-loading", t).removeClass("k-i-reload")
            },
            text: function(e, n) {
                var i = this.dataItem(e),
                    o = this.options[Q.text],
                    r = i.level(),
                    s = o.length,
                    a = o[Math.min(r, s - 1)];
                return n ? (i.set(a, n), t) : i[a]
            },
            _objectOrSelf: function(t) {
                return e(t).closest("[data-role=treeview]").data("kendoTreeView") || this
            },
            _dataSourceMove: function(t, n, i, o) {
                var r, s = this._objectOrSelf(i || n),
                    a = s.dataSource,
                    l = e.Deferred().resolve().promise();
                return i && i[0] != s.element[0] && (r = s.dataItem(i), r.loaded() || (s._progress(i, !0), l = r.load()), i != this.root && (a = r.children, a && a instanceof w || (r._initChildren(), r.loaded(!0), a = r.children))), t = this._toObservableData(t), o.call(s, a, t, l)
            },
            _toObservableData: function(t) {
                var n, i, o = t;
                return (X(t) || Z(t)) && (n = this._objectOrSelf(t).dataSource, i = e(t).attr(p.attr("uid")), o = n.getByUid(i), o && (o = n.remove(o))), o
            },
            _insert: function(e, t, n) {
                t instanceof p.data.ObservableArray ? t = t.toJSON() : _(t) || (t = [t]);
                var i = e.parent();
                return i && i._initChildren && (i.hasChildren = !0, i._initChildren()), e.splice.apply(e, [n, 0].concat(t)), this.findByUid(e[n].uid)
            },
            insertAfter: r(1),
            insertBefore: r(0),
            append: function(t, n, i) {
                var o = this.root;
                if (!(n && t instanceof jQuery && n[0] === t[0])) return n = n && n.length ? n : null, n && (o = c(n)), this._dataSourceMove(t, o, n, function(t, o, r) {
                    function s() {
                        n && l._expanded(n, !0, !0);
                        var e = t.data(),
                            i = Math.max(e.length, 0);
                        return l._insert(e, o, i)
                    }
                    var a, l = this;
                    return r.done(function() {
                        a = s(), (i = i || e.noop)(a)
                    }), a || null
                })
            },
            _remove: function(t, n) {
                var i, o, r, s = this;
                return t = e(t, s.element), this.angular("cleanup", function() {
                    return {
                        elements: t.get()
                    }
                }), i = t.parent().parent(), o = t.prev(), r = t.next(), t[n ? "detach" : "remove"](), i.hasClass("k-item") && (a(i), s._updateNodeClasses(i)), s._updateNodeClasses(o), s._updateNodeClasses(r), t
            },
            remove: function(e) {
                var t = this.dataItem(e);
                t && this.dataSource.remove(t)
            },
            detach: function(e) {
                return this._remove(e, !0)
            },
            findByText: function(t) {
                return e(this.element).find(".k-in").filter(function(n, i) {
                    return e(i).text() == t
                }).closest(q)
            },
            findByUid: function(t) {
                var n, i, o = this.element.find(".k-item"),
                    r = p.attr("uid");
                for (i = 0; i < o.length; i++)
                    if (o[i].getAttribute(r) == t) {
                        n = o[i];
                        break
                    }
                return e(n)
            },
            expandPath: function(t, n) {
                function i() {
                    s.shift(), s.length ? o(s[0]).then(i) : a.call(r)
                }

                function o(t) {
                    var n = e.Deferred(),
                        i = r.dataSource.get(t);
                    return i ? i.loaded() ? (i.set("expanded", !0), n.resolve()) : (r._progress(r.findByUid(i.uid), !0), i.load().then(function() {
                        i.set("expanded", !0), n.resolve()
                    })) : n.resolve(), n.promise()
                }
                var r = this,
                    s = t.slice(0),
                    a = n || e.noop;
                o(s[0]).then(i)
            },
            _parentIds: function(e) {
                for (var t = e && e.parentNode(), n = []; t && t.parentNode;) n.unshift(t.id), t = t.parentNode();
                return n
            },
            expandTo: function(e) {
                e instanceof p.data.Node || (e = this.dataSource.get(e));
                var t = this._parentIds(e);
                this.expandPath(t)
            },
            _renderItem: function(e) {
                return e.group || (e.group = {}), e.treeview = this.options, e.r = this.templates, this.templates.item(e)
            },
            _renderGroup: function(e) {
                var t = this;
                return e.renderItems = function(e) {
                    var n = "",
                        i = 0,
                        o = e.items,
                        r = o ? o.length : 0,
                        s = e.group;
                    for (s.length = r; i < r; i++) e.group = s, e.item = o[i], e.item.index = i, n += t._renderItem(e);
                    return n
                }, e.r = t.templates, t.templates.group(e)
            }
        }), f.plugin(l)
    }(window.kendo.jQuery), window.kendo
}, "function" == typeof define && define.amd ? define : function(e, t, n) {
    (n || t)()
}),
function(e, define) {
    define("dropdowntree/treeview.min", ["kendo.treeview.min"], e)
}(function() {
    return function(e, t) {
        function n(e) {
            return function(t) {
                var n = t.children(".k-animation-container");
                return n.length || (n = t), n.children(e)
            }
        }
        var i = window.kendo,
            o = i.ui,
            r = i.keys,
            s = "k-state-disabled",
            a = "select",
            l = "checked",
            c = "dataBound",
            d = "indeterminate",
            u = "navigate",
            h = o.TreeView,
            p = n(".k-group"),
            f = h.extend({
                init: function(e, t, n) {
                    var i = this;
                    i.dropdowntree = n, h.fn.init.call(i, e, t)
                },
                _checkOnSelect: function(e) {
                    if (!e.isDefaultPrevented()) {
                        var t = this.dataItem(e.node);
                        t.set("checked", !t.checked)
                    }
                },
                _click: function(e) {
                    var t = this;
                    t.dropdowntree._isMultipleSelection() && t.one("select", t._checkOnSelect), h.fn._click.call(t, e)
                },
                defaultrefresh: function(e) {
                    var n, i, o = e.node,
                        r = e.action,
                        s = e.items,
                        a = this.wrapper,
                        l = this.options,
                        d = l.loadOnDemand,
                        u = l.checkboxes && l.checkboxes.checkChildren;
                    if (!this._skip) {
                        if (e.field) {
                            if (!s[0] || !s[0].level) return;
                            return this._updateNodes(s, e.field)
                        }
                        if (o && (a = this.findByUid(o.uid), this._progress(a, !1)), u && "remove" != r) {
                            for (i = !1, n = 0; n < s.length; n++)
                                if ("checked" in s[n]) {
                                    i = !0;
                                    break
                                }
                            if (!i && o && o.checked)
                                for (n = 0; n < s.length; n++) s[n].checked = !0
                        }
                        if ("add" == r ? this._appendItems(e.index, s, a) : "remove" == r ? this._remove(this.findByUid(s[0].uid), !1) : "itemchange" == r ? this._updateNodes(s) : "itemloaded" == r ? this._refreshChildren(a, s, e.index) : this._refreshRoot(s), "remove" != r)
                            for (n = 0; n < s.length; n++) d && !s[n].expanded || s[n].load();
                        this.trigger(c, {
                            node: o ? a : t
                        }), this.dropdowntree._treeViewDataBound({
                            node: o ? a : t,
                            sender: this
                        }), this.options.checkboxes.checkChildren && this.updateIndeterminate()
                    }
                },
                _previousVisible: function(e) {
                    var t, n, i = this;
                    if (!e.length || e.prev().length)
                        for (n = e.length ? e.prev() : i.root.children().last(); i._expanded(n) && (t = p(n).children().last(), t.length);) n = t;
                    else n = i.parent(e) || e, n.length || (i.dropdowntree.checkAll && i.dropdowntree.checkAll.is(":visible") ? i.dropdowntree.checkAll.find(".k-checkbox").focus() : i.dropdowntree.filterInput ? i.dropdowntree.filterInput.focus() : i.dropdowntree.wrapper.focus());
                    return n
                },
                _keydown: function(n) {
                    var o, c = this,
                        h = n.keyCode,
                        p = c.current(),
                        f = c._expanded(p),
                        m = p.find(".k-checkbox-wrapper:first :checkbox"),
                        g = i.support.isRtl(c.element);
                    n.target == n.currentTarget && (!g && h == r.RIGHT || g && h == r.LEFT ? f ? o = c._nextVisible(p) : p.find(".k-in:first").hasClass(s) || c.expand(p) : !g && h == r.LEFT || g && h == r.RIGHT ? f && !p.find(".k-in:first").hasClass(s) ? c.collapse(p) : (o = c.parent(p), c._enabled(o) || (o = t)) : h == r.DOWN ? o = c._nextVisible(p) : h != r.UP || n.altKey ? h == r.HOME ? o = c._nextVisible(e()) : h == r.END ? o = c._previousVisible(e()) : h != r.ENTER || p.find(".k-in:first").hasClass(s) ? h == r.SPACEBAR && m.length && !p.find(".k-in:first").hasClass(s) ? (m.prop(l, !m.prop(l)).data(d, !1).prop(d, !1), c._checkboxChange({
                        target: m
                    }), o = p) : (n.altKey && h === r.UP || h === r.ESC) && c._closePopup() : p.find(".k-in:first").hasClass("k-state-selected") || c._trigger(a, p) || c.select(p) : o = c._previousVisible(p), o && (n.preventDefault(), p[0] != o[0] && (c._trigger(u, o), c.current(o))))
                },
                _closePopup: function() {
                    this.dropdowntree.close(), this.dropdowntree.wrapper.focus()
                },
                refresh: function(e) {
                    this.defaultrefresh(e), this.dropdowntree.options.skipUpdateOnBind || ("itemchange" === e.action ? this.dropdowntree._isMultipleSelection() ? "checked" === e.field && this.dropdowntree._checkValue(e.items[0]) : "checked" !== e.field && "expanded" !== e.field && e.items[0].selected && this.dropdowntree._selectValue(e.items[0]) : this.dropdowntree.refresh(e))
                }
            });
        i.ui._dropdowntree = f
    }(window.kendo.jQuery), window.kendo
}, "function" == typeof define && define.amd ? define : function(e, t, n) {
    (n || t)()
}),
function(e, define) {
    define("kendo.dropdowntree.min", ["dropdowntree/treeview.min", "kendo.popup.min"], e)
}(function() {
    return function(e, t) {
        function n(e, t, n) {
            for (var i, o = 0, r = t.length - 1; o < r; ++o) i = t[o], i in e || (e[i] = {}), e = e[i];
            e[t[r]] = n
        }
        var i, o, r = window.kendo,
            s = r.ui,
            a = s.Widget,
            l = s._dropdowntree,
            c = r.data.ObservableArray,
            d = r.data.ObservableObject,
            u = e.extend,
            h = r._activeElement,
            p = ".kendoDropDownTree",
            f = r.keys,
            m = r.support,
            g = "k-hidden",
            v = "width",
            _ = m.browser,
            b = r._outerWidth,
            w = ".",
            y = "disabled",
            k = "readonly",
            x = "k-state-disabled",
            C = "aria-disabled",
            S = "k-state-hover",
            T = "k-state-focused",
            D = "mouseenter" + p + " mouseleave" + p,
            A = "tabindex",
            E = "click",
            I = "open",
            M = "close",
            R = "change",
            F = e.proxy,
            P = r.ui.Widget.extend({
                init: function(t, n) {
                    var i, o, s;
                    this.ns = p, r.ui.Widget.fn.init.call(this, t, n), this._selection = this._getSelection(), this._focusInputHandler = e.proxy(this._focusInput, this), this._initial = this.element.val(), this._values = [], i = this.options.value, null !== i && i.length || (this._noInitialValue = !0), i && (this._valueMethodCalled = !0, this._values = e.isArray(i) ? i.slice(0) : [i]), this._inputTemplate(), this._accessors(), this._setTreeViewOptions(this.options), this._dataSource(), this._selection._initWrapper(), this._placeholder(!0), this._tabindex(), this.wrapper.data(A, this.wrapper.attr(A)), this.tree = e("<div/>").attr({
                        tabIndex: -1,
                        "aria-hidden": !0
                    }), this.list = e("<div class='k-list-container'/>").append(this.tree), this._header(), this._noData(), this._footer(), this._reset(), this._popup(), this.popup.one("open", F(this._popupOpen, this)), this._clearButton(), this._filterHeader(), this._treeview(), this._renderFooter(), this._checkAll(), this._enable(), this._toggleCloseVisibility(), this.options.autoBind || (o = n.text || "", n.value ? this._preselect(n.value) : o ? this._textAccessor(o) : n.placeholder && this._placeholder(!0)), s = e(this.element).parents("fieldset").is(":disabled"), s && this.enable(!1), this._valueMethodCalled = !1, r.notify(this)
                },
                _preselect: function(e, t) {
                    this._selection._preselect(e, t)
                },
                _setTreeViewOptions: function(t) {
                    var n = {
                        autoBind: t.autoBind,
                        checkboxes: t.checkboxes,
                        dataImageUrlField: t.dataImageUrlField,
                        dataSpriteCssClassField: t.dataSpriteCssClassField,
                        dataTextField: t.dataTextField,
                        dataUrlField: t.dataUrlField,
                        loadOnDemand: t.loadOnDemand,
                        idValid:t.idValid
                    };
                    this.options.treeview = e.extend({}, n, this.options.treeview), t.template && (this.options.treeview.template = t.template)
                },
                _dataSource: function() {
                    var t = this.options.dataSource;
                    this.dataSource = r.data.HierarchicalDataSource.create(t), t && e.extend(this.options.treeview, {
                        dataSource: this.dataSource
                    })
                },
                _popupOpen: function() {
                    var e = this.popup;
                    e.wrapper = r.wrap(e.element)
                },
                _getSelection: function() {
                    return this._isMultipleSelection() ? new s.DropDownTree.MultipleSelection(this) : new s.DropDownTree.SingleSelection(this)
                },
                setDataSource: function(e) {
                    this.dataSource = e, this.treeview.setDataSource(e)
                },
                _isMultipleSelection: function() {
                    return this.options && (this.options.treeview.checkboxes || this.options.checkboxes)
                },
                options: {
                    name: "DropDownTree",
                    animation: {},
                    autoBind: !0,
                    autoClose: !0,
                    autoWidth: !1,
                    clearButton: !0,
                    dataTextField: "",
                    dataValueField: "",
                    dataImageUrlField: "",
                    dataSpriteCssClassField: "",
                    dataUrlField: "",
                    delay: 500,
                    enabled: !0,
                    enforceMinLength: !1,
                    filter: "none",
                    height: 200,
                    ignoreCase: !0,
                    index: 0,
                    loadOnDemand: !1,
                    messages: {
                        singleTag: "item(s) selected",
                        clear: "clear",
                        deleteTag: "delete"
                    },
                    minLength: 1,
                    checkboxes: !1,
                    noDataTemplate: "No data found.",
                    placeholder: "",
                    checkAll: !1,
                    checkAllTemplate: "Check all",
                    tagMode: "multiple",
                    template: null,
                    text: null,
                    treeview: {},
                    valuePrimitive: !1,
                    footerTemplate: "",
                    headerTemplate: "",
                    value: null,
                    valueTemplate: null
                },
                events: ["open", "close", "dataBound", R, "select", "filtering"],
                focus: function() {
                    this.wrapper.focus()
                },
                dataItem: function(e) {
                    return this.treeview.dataItem(e)
                },
                readonly: function(e) {
                    this._editable({
                        readonly: e === t || e,
                        disable: !1
                    }), this._toggleCloseVisibility()
                },
                enable: function(e) {
                    this._editable({
                        readonly: !1,
                        disable: !(e = e === t || e)
                    }), this._toggleCloseVisibility()
                },
                toggle: function(e) {
                    this._toggle(e)
                },
                open: function() {
                    var e = this.popup;
                    this.options.autoBind || this.dataSource.data().length || (this.treeview._progress(!0), this._isFilterEnabled() ? this._search() : this.dataSource.fetch()), !e.visible() && this._allowOpening() && (this._isMultipleSelection() && e.element.addClass("k-multiple-selection"), e.element.addClass("k-popup-dropdowntree"), e.one("activate", this._focusInputHandler), e._hovered = !0, e.open())
                },
                close: function() {
                    this.popup.close()
                },
                search: function(t) {
                    var n, i = this.options;
                    if (clearTimeout(this._typingTimeout), !i.enforceMinLength && !t.length || t.length >= i.minLength) {
                        if (n = this._getFilter(t), this.trigger("filtering", {
                                filter: n
                            }) || e.isArray(this.options.dataTextField)) return;
                        this._filtering = !0, this.treeview.dataSource.filter(n)
                    }
                },
                _getFilter: function(e) {
                    return {
                        field: this.options.dataTextField,
                        operator: this.options.filter,
                        value: e,
                        ignoreCase: this.options.ignoreCase
                    }
                },
                refresh: function() {
                    var t = this.treeview.dataSource.flatView();
                    this._renderFooter(), this._renderNoData(), this.filterInput && this.checkAll && this.checkAll.toggle(!this.filterInput.val().length), this.tree.toggle(!!t.length), e(this.noData).toggle(!t.length)
                },
                setOptions: function(e) {
                    a.fn.setOptions.call(this, e), this._setTreeViewOptions(e), this._dataSource(), this.options.treeview && this.treeview.setOptions(this.options.treeview), e.height && this.tree && this.tree.css("max-height", e.height), this._header(), this._noData(), this._footer(), this._renderFooter(), this._renderNoData(), this.span && (this._isMultipleSelection() || this.span.hasClass("k-readonly")) && this._placeholder(!0), this._inputTemplate(), this._accessors(), this._filterHeader(), this._checkAll(), this._enable(), e && (e.enable || e.enabled) && this.enable(!0), this._clearButton()
                },
                destroy: function() {
                    r.ui.Widget.fn.destroy.call(this), this.treeview && this.treeview.destroy(), this.popup.destroy(), this.wrapper.off(p), this._clear.off(p), this._inputWrapper.off(p), this.filterInput && this.filterInput.off(p), this.tagList && this.tagList.off(p), r.unbind(this.tagList), this.options.checkAll && this.checkAll && this.checkAll.off(p), this._form && this._form.off("reset", this._resetHandler)
                },
                setValue: function(t) {
                    t = e.isArray(t) || t instanceof c ? t.slice(0) : [t], this._values = t
                },
                items: function() {
                    this.treeview.dataItems()
                },
                value: function(e) {
                    var n = this;
                    if (e)
                        if (n.filterInput && n.dataSource._filter) n._filtering = !0, n.dataSource.filter({});
                        else if (!n.dataSource.data().length) return n.dataSource.fetch(function() {
                        n._selection._setValue(e)
                    }), t;
                    return n._selection._setValue(e)
                },
                text: function(e) {
                    var n, i = this.options.ignoreCase;
                    return e = null === e ? "" : e, e === t || this._isMultipleSelection() ? this._textAccessor() : "string" != typeof e ? (this._textAccessor(e), t) : (n = i ? e : e.toLowerCase(), this._selectItemByText(n), this._textAccessor(n), t)
                },
                _header: function() {
                    var n, i = this,
                        o = e(i.header),
                        s = i.options.headerTemplate;
                    return this._angularElement(o, "cleanup"), r.destroy(o), o.remove(), s ? (n = "function" != typeof s ? r.template(s) : s, o = e(n({})), i.header = o[0] ? o : null, i.list.prepend(o), this._angularElement(i.header, "compile"), t) : (i.header = null, t)
                },
                _noData: function() {
                    var n = this,
                        i = e(n.noData),
                        o = n.options.noDataTemplate;
                    return n.angular("cleanup", function() {
                        return {
                            elements: i
                        }
                    }), r.destroy(i), i.remove(), o ? (n.noData = e('<div class="k-nodata" style="display:none"><div></div></div>').appendTo(n.list), n.noDataTemplate = "function" != typeof o ? r.template(o) : o, t) : (n.noData = null, t)
                },
                _renderNoData: function() {
                    var e = this,
                        t = e.noData;
                    t && (this._angularElement(t, "cleanup"), t.children(":first").html(e.noDataTemplate({
                        instance: e
                    })), this._angularElement(t, "compile"))
                },
                _footer: function() {
                    var n = this,
                        i = e(n.footer),
                        o = n.options.footerTemplate;
                    return this._angularElement(i, "cleanup"), r.destroy(i), i.remove(), o ? (n.footer = e('<div class="k-footer"></div>').appendTo(n.list), n.footerTemplate = "function" != typeof o ? r.template(o) : o, t) : (n.footer = null, t)
                },
                _renderFooter: function() {
                    var e = this,
                        t = e.footer;
                    t && (this._angularElement(t, "cleanup"), t.html(e.footerTemplate({
                        instance: e
                    })), this._angularElement(t, "compile"))
                },
                _enable: function() {
                    var e = this,
                        n = e.options,
                        i = e.element.is("[disabled]");
                    n.enable !== t && (n.enabled = n.enable), !n.enabled || i ? e.enable(!1) : e.readonly(e.element.is("[readonly]"))
                },
                _adjustListWidth: function() {
                    var e, t, n = this,
                        i = n.list,
                        o = i[0].style.width,
                        r = n.wrapper;
                    if (i.data(v) || !o) return e = window.getComputedStyle ? window.getComputedStyle(r[0], null) : 0, t = parseFloat(e && e.width) || b(r), e && _.msie && (t += parseFloat(e.paddingLeft) + parseFloat(e.paddingRight) + parseFloat(e.borderLeftWidth) + parseFloat(e.borderRightWidth)), o = "border-box" !== i.css("box-sizing") ? t - (b(i) - i.width()) : t, i.css({
                        fontFamily: r.css("font-family"),
                        width: n.options.autoWidth ? "auto" : o,
                        minWidth: o,
                        whiteSpace: n.options.autoWidth ? "nowrap" : "normal"
                    }).data(v, o), !0
                },
                _reset: function() {
                    var t = this,
                        n = t.element,
                        i = n.attr("form"),
                        o = i ? e("#" + i) : n.closest("form");
                    o[0] && (t._resetHandler = function() {
                        setTimeout(function() {
                            t.value(t._initial)
                        })
                    }, t._form = o.on("reset", t._resetHandler))
                },
                _popup: function() {
                    var e = this;
                    e.popup = new s.Popup(e.list, u({}, e.options.popup, {
                        anchor: e.wrapper,
                        open: F(e._openHandler, e),
                        close: F(e._closeHandler, e),
                        animation: e.options.animation,
                        isRtl: m.isRtl(e.wrapper),
                        autosize: e.options.autoWidth
                    }))
                },
                _angularElement: function(e, t) {
                    e && this.angular(t, function() {
                        return {
                            elements: e
                        }
                    })
                },
                _allowOpening: function() {
                    return this.options.noDataTemplate || this.treeview.dataSource.flatView().length
                },
                _placeholder: function(e) {
                    this.span && this.span.toggleClass("k-readonly", e).text(e ? this.options.placeholder : "");
                },
                _currentValue: function(e) {
                    var t = this._value(e);
                    return t || 0 === t || (t = e), t
                },
                _checkValue: function(n) {
                    var i, o, r, s, a = "",
                        l = -1,
                        c = this.value(),
                        u = "multiple" === this.options.tagMode;
                    if ((n || 0 === n) && (n.level && (n._level = n.level()), a = this._currentValue(n), l = c.indexOf(a)), n.checked) {
                        if (i = e.grep(this._tags, function(e) {
                                return e.uid === n._tagUid
                            }), i.length) return;
                        o = new d(n.toJSON()), n._tagUid = o.uid, this._tags.push(o), 1 === this._tags.length && (this.span.hide(), u || this._multipleTags.push(o)), l === -1 && (c.push(a), this.setValue(c))
                    } else {
                        if (r = this._tags.find(function(e) {
                                return e.uid === n._tagUid
                            }), s = this._tags.indexOf(r), s === -1) return this._treeViewCheckAllCheck(n), t;
                        this._tags.splice(s, 1), 0 === this._tags.length && (this.span.show(), u || this._multipleTags.splice(0, 1)), l !== -1 && (c.splice(l, 1), this.setValue(c))
                    }
                    this._treeViewCheckAllCheck(n), this._preventChangeTrigger || this._valueMethodCalled || this._noInitialValue || this.trigger(R), this.options.autoClose && this.popup.visible() && (this.close(), this.wrapper.focus()), this.popup.position(), this._toggleCloseVisibility()
                },
                _selectValue: function(e) {
                    if(e.value){
                    var t = "",
                        n = "";
                    (e || 0 === e) && (e.level && (e._level = e.level()), n = this._text(e) || e, t = this._currentValue(e)), null === t && (t = ""), this.setValue(t), this._textAccessor(n, e), this._accessor(t), this._valueMethodCalled || this.trigger(R), this._valueMethodCalled = !1, this.options.autoClose && this.popup.visible() && (this.close(), this.wrapper.focus()), this.popup.position(), this._toggleCloseVisibility()
                }
                },
                _clearClick: function(e) {
                    e.stopPropagation(), this._clearTextAndValue()
                },
                _clearTextAndValue: function() {
                    this.setValue([]), this._clearInput(), this._clearText(), this._selection._clearValue(), this.popup.position(), this._toggleCloseVisibility()
                },
                _clearText: function() {
                    this.options.placeholder ? this._placeholder(!0) : this.span && this.span.val("")
                },
                _inputTemplate: function() {
                    var t = this.options.valueTemplate;
                    t = t ? r.template(t) : e.proxy(r.template("#:this._text(data)#", {
                        useWithBlock: !1
                    }), this), this.valueTemplate = t
                },
                _assignInstance: function(e, t) {
                    var i = this.options.dataTextField,
                        o = {};
                    return i ? (n(o, i.split(w), e), n(o, this.options.dataValueField.split(w), t), o = new d(o)) : o = e, o
                },
                _textAccessor: function(n, i) {
                    var o, r = this.valueTemplate,
                        s = this.span;
                    if (n === t) return s.text();
                    s.removeClass("k-readonly"), !i && (e.isPlainObject(n) || n instanceof d) && (i = n), i || (i = this._assignInstance(n, this._accessor())), o = function() {
                        return {
                            elements: s.get(),
                            data: [{
                                dataItem: i
                            }]
                        }
                    }, this.angular("cleanup", o);
                    try {
                        var text = r(i).replace("&#39;", "'");
                        s.val(text)
                    } catch (a) {
                        s && s.val("")
                    }
                    this.angular("compile", o)
                },
                _accessors: function() {
                    var t = this.element,
                        n = this.options,
                        i = r.getter,
                        o = t.attr(r.attr("text-field")),
                        s = t.attr(r.attr("value-field")),
                        a = function(t) {
                            var n, o;
                            return e.isArray(t) ? (n = t.length, o = e.map(t, function(e) {
                                return function(t) {
                                    return t[e]
                                }
                            }), function(e) {
                                var t = e._level;
                                if (t || 0 === t) return o[Math.min(t, n - 1)](e)
                            }) : i(t)
                        };
                    !n.dataTextField && o && (n.dataTextField = o), !n.dataValueField && s && (n.dataValueField = s), n.dataTextField = n.dataTextField || "text", n.dataValueField = n.dataValueField || "value", this._text = a(n.dataTextField), this._value = a(n.dataValueField)
                },
                _accessor: function(e, t) {
                    return this._accessorInput(e, t)
                },
                _accessorInput: function(e) {
                    var n = this.element[0];
                    return e === t ? n.value : (null === e && (e = ""), n.value = e, t)
                },
                _clearInput: function() {
                    var e = this.element[0];
                    e.value = ""
                },
                _clearButton: function() {
                    var t = this.options.messages && this.options.messages.clear ? this.options.messages.clear : "clear";
                    this._clear || (this._clear = e('<span unselectable="on" class="k-icon k-clear-value k-i-close" title="' + t + '"></span>').attr({
                        role: "button",
                        tabIndex: -1
                    })), this.options.clearButton ? (this._clear.insertAfter(this.span), this.wrapper.addClass("k-dropdowntree-clearable")) : this.options.clearButton || this._clear.remove()
                },
                _toggleCloseVisibility: function() {
                    var e = this.element.attr(k),
                        t = this.value() && !this._isMultipleSelection() || this.value().length,
                        n = this.element.val() && this.element.val() !== this.options.placeholder;
                    e || !t && !n ? this._hideClear() : this._showClear()
                },
                _showClear: function() {
                    this._clear && this._clear.removeClass(g)
                },
                _hideClear: function() {
                    this._clear && this._clear.addClass(g)
                },
                _openHandler: function(e) {
                    this._adjustListWidth(), this.trigger(I) ? e.preventDefault() : (this.wrapper.attr("aria-expanded", !0), this.tree.attr("aria-hidden", !1).attr("role", "tree"))
                },
                _closeHandler: function(e) {
                    this.trigger(M) ? e.preventDefault() : (this.wrapper.attr("aria-expanded", !1), this.tree.attr("aria-hidden", !0))
                },
                _treeview: function() {
                    var e = this;
                    e.options.height && e.tree.css("max-height", e.options.height), e.tree.attr("id", r.guid()), e.treeview = new l(e.tree, u({
                        select: e.options.select
                    }, e.options.treeview), e), e.dataSource = e.treeview.dataSource
                },
                _treeViewDataBound: function(e) {
                    var n, i, o;
                    return e.node && this._prev && this._prev.length && e.sender.expand(e.node), this._filtering ? (e.node || (this._filtering = !1), this._isMultipleSelection() || this._deselectItem(e), t) : (this.treeview || (this.treeview = e.sender), e.node ? (i = e.sender.dataItem(e.node), i && (o = i.children.data(), this._checkLoadedItems(o))) : (n = e.sender.dataSource.data(), this._checkLoadedItems(n), this._noInitialValue && (this._noInitialValue = !1)), this.trigger("dataBound", e), t)
                },
                _deselectItem: function(e) {
                    var t, n, i = [];
                    for (e.node ? (t = e.sender.dataItem(e.node), t && (i = t.children.data())) : i = e.sender.dataSource.data(), n = 0; n < i.length; n++) i[n].selected && !this._valueComparer(i[n], this.value()) && i[n].set("selected", !1)
                },
                _checkLoadedItems: function(e) {
                    var t, n = this.value();
                    if (e)
                        for (t = 0; t < e.length; t++) this._selection._checkLoadedItem(e[t], n)
                },
                _treeViewCheckAllCheck: function(e) {
                    this.options.checkAll && this.checkAll && (this._getAllChecked(), e.checked ? this._checkCheckAll() : this._uncheckCheckAll())
                },
                _checkCheckAll: function() {
                    var e = this.checkAll.find(".k-checkbox");
                    this._allItemsAreChecked ? e.prop("checked", !0).prop("indeterminate", !1) : e.prop("indeterminate", !0)
                },
                _uncheckCheckAll: function() {
                    var e = this.checkAll.find(".k-checkbox");
                    this._allItemsAreUnchecked ? e.prop("checked", !1).prop("indeterminate", !1) : e.prop("indeterminate", !0)
                },
                _filterHeader: function() {
                    var t;
                    this.filterInput && (this.filterInput.off(p).parent().remove(), this.filterInput = null), this._isFilterEnabled() && (this._disableCheckChildren(), t = '<span class="k-icon k-i-zoom"></span>', this.filterInput = e('<input class="k-textbox"/>').attr({
                        placeholder: this.element.attr("placeholder"),
                        title: this.element.attr("title"),
                        role: "listbox",
                        "aria-haspopup": !0,
                        "aria-expanded": !1
                    }), this.filterInput.on("input", F(this._filterChange, this)), e('<span class="k-list-filter" />').insertBefore(this.tree).append(this.filterInput.add(t)))
                },
                _filterChange: function() {
                    this.filterInput && this._search()
                },
                _disableCheckChildren: function() {
                    this._isMultipleSelection() && this.options.treeview.checkboxes && this.options.treeview.checkboxes.checkChildren && (this.options.treeview.checkboxes.checkChildren = !1)
                },
                _checkAll: function() {
                    this.checkAll && (this.checkAll.find(".k-checkbox-label, .k-checkbox").off(p), this.checkAll.remove(), this.checkAll = null), this._isMultipleSelection() && this.options.checkAll && (this.checkAll = e('<div class="k-check-all"><input type="checkbox" class="k-checkbox"/><span class="k-checkbox-label">Check All</span></div>').insertBefore(this.tree), this.checkAll.find(".k-checkbox-label").html(r.template(this.options.checkAllTemplate)({
                        instance: this
                    })), this.checkAll.find(".k-checkbox-label").on(E + p, F(this._clickCheckAll, this)), this.checkAll.find(".k-checkbox").on("change" + p, F(this._changeCheckAll, this)).on("keydown" + p, F(this._keydownCheckAll, this)), this._disabledCheckedItems = [], this._disabledUnCheckedItems = [], this._getAllChecked(), this._allItemsAreUnchecked || this._checkCheckAll())
                },
                _changeCheckAll: function() {
                    var e = this.checkAll.find(".k-checkbox"),
                        t = e.prop("checked");
                    _.msie || _.edge || this._updateCheckAll(t)
                },
                _updateCheckAll: function(e) {
                    var t = this.checkAll.find(".k-checkbox");
                    this._toggleCheckAllItems(e), t.prop("checked", e), this._disabledCheckedItems.length && this._disabledUnCheckedItems.length ? t.prop("indeterminate", !0) : this._disabledCheckedItems.length ? t.prop("indeterminate", !e) : this._disabledUnCheckedItems.length ? t.prop("indeterminate", e) : t.prop("indeterminate", !1), this._disabledCheckedItems = [], this._disabledUnCheckedItems = []
                },
                _keydownCheckAll: function(e) {
                    var n = e.keyCode,
                        i = e.altKey;
                    return i && n === f.UP || n === f.ESC ? (this.close(), this.wrapper.focus(), e.preventDefault(), t) : (n === f.UP && (this.filterInput ? this.filterInput.focus() : this.wrapper.focus(), e.preventDefault()), n === f.DOWN && (this.tree && this.tree.is(":visible") && this.tree.focus(), e.preventDefault()), n === f.SPACEBAR && (_.msie || _.edge) && (this._clickCheckAll(), e.preventDefault()), t)
                },
                _clickCheckAll: function() {
                    var e = this.checkAll.find(".k-checkbox"),
                        t = e.prop("checked");
                    this._updateCheckAll(!t), e.focus()
                },
                _dfs: function(e, t, n) {
                    for (var i = 0; i < e.length && this[t](e[i], n); i++) this._traverceChildren(e[i], t, n)
                },
                _uncheckItemByUid: function(e) {
                    this._dfs(this.dataSource.data(), "_uncheckItemEqualsUid", e)
                },
                _uncheckItemEqualsUid: function(e, t) {
                    return e.enabled === !1 || e._tagUid != t || (e.set("checked", !1), !1)
                },
                _selectItemByText: function(e) {
                    this._dfs(this.dataSource.data(), "_itemEqualsText", e)
                },
                _itemEqualsText: function(e, t) {
                    return e.enabled === !1 || this._text(e) !== t || (this.treeview.select(this.treeview.findByUid(e.uid)), this._selectValue(e), !1)
                },
                _selectItemByValue: function(e) {
                    this._dfs(this.dataSource.data(), "_itemEqualsValue", e)
                },
                _itemEqualsValue: function(e, t) {
                    return e.enabled === !1 || !this._valueComparer(e, t) || (this.treeview.select(this.treeview.findByUid(e.uid)), !1)
                },
                _checkItemByValue: function(e) {
                    var t, n = this.treeview.dataItems();
                    for (t = 0; t < e.length; t++) this._dfs(n, "_checkItemEqualsValue", e[t])
                },
                _checkItemEqualsValue: function(e, t) {
                    return e.enabled === !1 || !this._valueComparer(e, t) || (e.set("checked", !0), !1)
                },
                _valueComparer: function(e, t) {
                    var n, i, o = this._value(e);
                    return o ? !!t && (i = this._value(t), i ? o == i : o == t) : (n = this._text(e), !!n && (this._text(t) ? n == this._text(t) : n == t))
                },
                _getAllChecked: function() {
                    return this._allCheckedItems = [], this._allItemsAreChecked = !0, this._allItemsAreUnchecked = !0, this._dfs(this.dataSource.data(), "_getAllCheckedItems"), this._allCheckedItems
                },
                _getAllCheckedItems: function(e) {
                    return this._allItemsAreChecked && (this._allItemsAreChecked = e.checked), this._allItemsAreUnchecked && (this._allItemsAreUnchecked = !e.checked), e.checked && this._allCheckedItems.push(e), !0
                },
                _traverceChildren: function(e, t, n) {
                    var i = e._childrenOptions && e._childrenOptions.schema ? e._childrenOptions.schema.data : null,
                        o = e[i] || e.items || e.children;
                    o && this._dfs(o, t, n)
                },
                _toggleCheckAllItems: function(e) {
                    this._dfs(this.dataSource.data(), "_checkAllCheckItem", e)
                },
                _checkAllCheckItem: function(e, t) {
                    return e.enabled === !1 ? e.checked ? this._disabledCheckedItems.push(e) : this._disabledUnCheckedItems.push(e) : e.set("checked", t), !0
                },
                _isFilterEnabled: function() {
                    var e = this.options.filter;
                    return e && "none" !== e
                },
                _editable: function(t) {
                    var n = this,
                        i = n.element,
                        o = t.disable,
                        r = t.readonly,
                        s = n.wrapper.add(n.filterInput).off(p),
                        a = n._inputWrapper.off(D);
                    n._isMultipleSelection() && n.tagList.off(E + p), r || o ? o ? (s.removeAttr(A), a.addClass(x)) : (s.attr(A, s.data(A)), a.removeClass(x), s.on("focusin" + p, F(n._focusinHandler, n)).on("focusout" + p, F(n._focusoutHandler, n))) : (i.removeAttr(y).removeAttr(k), a.removeClass(x).on(D, n._toggleHover), n._clear.on("click" + p, F(n._clearClick, n)), s.attr(A, s.data(A)).attr(C, !1).on("keydown" + p, F(n._keydown, n)).on("focusin" + p, F(n._focusinHandler, n)).on("focusout" + p, F(n._focusoutHandler, n)), n.wrapper.on(E + p, F(n._wrapperClick, n)), this._isMultipleSelection() && (n.tagList.on(E + p, "li.k-button", function(t) {
                        e(t.currentTarget).addClass(T)
                    }), n.tagList.on(E + p, ".k-select", function(e) {
                        n._removeTagClick(e)
                    }))), i.attr(y, o).attr(k, r), s.attr(C, o)
                },
                _focusinHandler: function() {
                    this._inputWrapper.addClass(T)
                },
                _focusoutHandler: function() {
                    this._inputWrapper.removeClass(T), this._isMultipleSelection() && this.tagList.find(w + T).removeClass(T)
                },
                _toggle: function(e) {
                    e = e !== t ? e : !this.popup.visible(), this[e ? I : M]()
                },
                _wrapperClick: function(e) {
                    e.preventDefault(), this.popup.unbind("activate", this._focusInputHandler), this._focused = this.wrapper, this._prevent = !1, this._toggle()
                },
                _toggleHover: function(t) {
                    e(t.currentTarget).toggleClass(S, "mouseenter" === t.type)
                },
                _focusInput: function() {
                    this.filterInput ? this.filterInput.focus() : this.checkAll ? this.checkAll.find(".k-checkbox").focus() : this.tree.is(":visible") && this.tree.focus()
                },
                _keydown: function(e) {
                    var n, i, o, r, s = e.keyCode,
                        a = e.altKey,
                        l = this.popup.visible();
                    if (this.filterInput && (n = this.filterInput[0] === h()), this.wrapper && (i = this.wrapper[0] === h()), i) {
                        if (s === f.ESC) return this._clearTextAndValue(), e.preventDefault(), t;
                        if (this._isMultipleSelection()) {
                            if (s === f.LEFT) return this._focusPrevTag(), e.preventDefault(), t;
                            if (s === f.RIGHT) return this._focusNextTag(), e.preventDefault(), t;
                            if (s === f.HOME) return this._focusFirstTag(), e.preventDefault(), t;
                            if (s === f.END) return this._focusLastTag(), e.preventDefault(), t;
                            if (s === f.DELETE) return o = this.tagList.find(w + T).first(), o.length && (r = this._tags[o.index()], this._removeTag(r)), e.preventDefault(), t;
                            if (s === f.BACKSPACE) return o = this.tagList.find(w + T).first(), o.length ? (r = this._tags[o.index()], this._removeTag(r)) : (o = this._focusLastTag(), o.length && (r = this._tags[o.index()], this._removeTag(r))), e.preventDefault(), t
                        }
                    }
                    return n && (s === f.ESC && this._clearFilter(), s !== f.UP || a || (this.wrapper.focus(), e.preventDefault()), _.msie && _.version < 10 && (s !== f.BACKSPACE && s !== f.DELETE || this._search())), a && s === f.UP || s === f.ESC ? (this.wrapper.focus(), this.close(), e.preventDefault(), t) : s === f.ENTER && this._typingTimeout && this.filterInput && l ? (e.preventDefault(), t) : (s !== f.SPACEBAR || n || (this._toggle(!l), e.preventDefault()), a && s === f.DOWN && !l && (this.open(), e.preventDefault()), s === f.DOWN && l && (this.filterInput && !n ? this.filterInput.focus() : this.checkAll && this.checkAll.is(":visible") ? this.checkAll.find("input").focus() : this.tree.is(":visible") && this.tree.focus(), e.preventDefault()), t)
                },
                _focusPrevTag: function() {
                    var e, t = this.tagList.find(w + T);
                    t.length ? (e = this.wrapper.attr("aria-activedescendant"), t.first().removeClass(T).removeAttr("id").prev().addClass(T).attr("id", e), this.wrapper.attr("aria-activedescendant", e)) : this._focusLastTag()
                },
                _focusNextTag: function() {
                    var e, t = this.tagList.find(w + T);
                    t.length ? (e = this.wrapper.attr("aria-activedescendant"), t.first().removeClass(T).removeAttr("id").next().addClass(T).attr("id", e), this.wrapper.attr("aria-activedescendant", e)) : this._focusFirstTag()
                },
                _focusFirstTag: function() {
                    var e, t = this.wrapper.attr("aria-activedescendant");
                    return this._clearDisabledTag(), e = this.tagList.children("li").first().addClass(T).attr("id", t), this.wrapper.attr("aria-activedescendant", t), e
                },
                _focusLastTag: function() {
                    var e, t = this.wrapper.attr("aria-activedescendant");
                    return this._clearDisabledTag(), e = this.tagList.children("li").last().addClass(T).attr("id", t), this.wrapper.attr("aria-activedescendant", t), e
                },
                _clearDisabledTag: function() {
                    this.tagList.find(w + T).removeClass(T).removeAttr("id")
                },
                _search: function() {
                    var e = this;
                    clearTimeout(e._typingTimeout), e._typingTimeout = setTimeout(function() {
                        var t = e.filterInput.val();
                        e._prev !== t && (e._prev = t, e.search(t)), e._typingTimeout = null
                    }, e.options.delay)
                },
                _clearFilter: function() {
                    this.filterInput.val().length && (this.filterInput.val(""), this._prev = "", this._filtering = !0, this.treeview.dataSource.filter({}))
                },
                _removeTagClick: function(t) {
                    t.stopPropagation();
                    var n = this._tags[e(t.currentTarget.parentElement).index()];
                    this._removeTag(n)
                },
                _removeTag: function(e) {
                    if (e) {
                        var t = e.uid;
                        this._uncheckItemByUid(t)
                    }
                }
            });
        s.plugin(P), i = r.Class.extend({
            init: function(e) {
                this._dropdowntree = e
            },
            _initWrapper: function() {
                this._wrapper(), this._span()
            },
            _preselect: function(e) {
                var t = this._dropdowntree;
                t._selectValue(e)
            },
            _wrapper: function() {
                var e, t = this._dropdowntree,
                    n = t.element,
                    i = n[0];
                e = n.parent(), e.is("span.k-widget") || (e = n.wrap("<span />").parent(), e[0].style.cssText = i.style.cssText, e[0].title = i.title), t._focused = t.wrapper = e.addClass("k-widget k-dropdowntree").addClass(i.className).css("display", "").attr({
                    accesskey: n.attr("accesskey"),
                    unselectable: "on",
                    role: "listbox",
                    "aria-haspopup": !0,
                    "aria-expanded": !1
                }), n.hide().removeAttr("accesskey")
            },
            _span: function() {
                var t, n = this._dropdowntree,
                    i = n.wrapper,
                    o = "input.k-input";
                t = i.find(o), t[0] || (i.append('<span unselectable="on" class="k-dropdown-wrap k-state-default"><input '+(n.options.idValid?('id='+n.options.idValid):'')+' readonly required unselectable="on" class="k-input">&nbsp;</input><span unselectable="on" class="k-select" aria-label="select"><i class="material-icons">arrow_drop_down</i></span></span>').append(n.element), t = i.find(o)), n.span = t, n._inputWrapper = e(i[0].firstChild), n._arrow = i.find(".k-select"), n._arrowIcon = n._arrow.find(".k-icon")
            },
            _setValue: function(e) {
                var n, i = this._dropdowntree;
                return e === t || null === e ? (n = i._values.slice()[0], e = "object" == typeof n ? n : i._accessor() || n, e === t || null === e ? "" : e) : (i._valueMethodCalled = !0, 0 === e.length ? (i._clearTextAndValue(), i._valueMethodCalled = !1, t) : (i._selectItemByValue(e), i._toggleCloseVisibility(), t))
            },
            _clearValue: function() {
                var e = this._dropdowntree,
                    t = e.treeview.select();
                e.treeview.dataItem(t) && (e.treeview.dataItem(t).set("selected", !1), e._valueMethodCalled || e.trigger(R))
            },
            _checkLoadedItem: function(e, t) {
                var n = this._dropdowntree;
                (t && n._valueComparer(e, t) || !t && e.selected) && n.treeview.select(n.treeview.findByUid(e.uid))
            }
        }), o = r.Class.extend({
            init: function(e) {
                this._dropdowntree = e
            },
            _initWrapper: function() {
                var t = this._dropdowntree;
                this._tagTemplate(), t.element.attr("multiple", "multiple").hide(), this._wrapper(), t._tags = new c([]), t._multipleTags = new c([]), this._tagList(), t.span = e('<input '+(n.options.idValid?('id='+n.options.idValid):'')+' readonly required unselectable="on" class="k-input">&nbsp;</input>').insertAfter(t.tagList), t._inputWrapper = e(t.wrapper[0].firstChild)
            },
            _preselect: function(t, n) {
                var i = this._dropdowntree,
                    o = n || i.options.value;
                e.isArray(t) || t instanceof r.data.ObservableArray || (t = [t]), (e.isPlainObject(t[0]) || t[0] instanceof r.data.ObservableObject || !i.options.dataValueField) && (i.dataSource.data(t), i.value(o))
            },
            _tagTemplate: function() {
                var e = this._dropdowntree,
                    t = e.options,
                    n = t.valueTemplate,
                    i = "multiple" === t.tagMode,
                    o = t.messages.singleTag;
                n = n ? r.template(n) : e.valueTemplate, e.valueTemplate = function(t) {
                    return i ? '<li class="k-button ' + (t.enabled === !1 ? "k-state-disabled" : "") + '" unselectable="on" role="option" ' + (t.enabled === !1 ? 'aria-disabled="true"' : "") + '><span unselectable="on">' + n(t) + '</span><span title="' + e.options.messages.deleteTag + '" aria-label="' + e.options.messages.deleteTag + '" class="k-select"><span class="k-icon k-i-close"></span></span></li>' : '<li class="k-button" unselectable="on" role="option"><span unselectable="on" data-bind="text: tags.length"></span><span unselectable="on">&nbsp;' + o + "</span></li>"
                }
            },
            _wrapper: function() {
                var t = this._dropdowntree,
                    n = t.element,
                    i = n.parent("span.k-dropdowntree");
                i[0] || (i = n.wrap('<div class="k-widget k-dropdowntree" unselectable="on" />').parent(), i[0].style.cssText = n[0].style.cssText, i[0].title = n[0].title, e('<div class="k-multiselect-wrap k-floatwrap" unselectable="on" />').insertBefore(n)), t.wrapper = i.addClass(n[0].className).css("display", "").attr({
                    role: "listbox",
                    "aria-activedescendant": r.guid(),
                    "aria-haspopup": !0,
                    "aria-expanded": !1
                }), t._innerWrapper = e(i[0].firstChild)
            },
            _tagList: function() {
                var t, n, i, o = this._dropdowntree,
                    s = o._innerWrapper.children("ul");
                s[0] || (t = "multiple" === o.options.tagMode, n = t ? "tags" : "multipleTag", s = e('<ul role="listbox" unselectable="on" data-template="tagTemplate" data-bind="source: ' + n + '" class="k-reset"/>').appendTo(o._innerWrapper)), o.tagList = s, o.tagList.attr("id", r.guid() + "_tagList"), o.wrapper.attr("aria-owns", o.tagList.attr("id")), i = r.observable({
                    multipleTag: o._multipleTags,
                    tags: o._tags,
                    tagTemplate: o.valueTemplate
                }), r.bind(o.tagList, i), o.tagList.attr("data-stop", !0)
            },
            _setValue: function(e) {
                var n = this._dropdowntree,
                    i = n._values;
                return e === t || null === e ? n._values.slice() : (n.setValue(e), n._valueMethodCalled = !0, e.length ? (this._removeValues(i, e), n._checkItemByValue(e)) : n._clearTextAndValue(), n._valueMethodCalled = !1, n._toggleCloseVisibility(), t)
            },
            _removeValues: function(e, t) {
                var n, i, o = this._dropdowntree,
                    r = this._getNewValues(e, t);
                for (n = 0; n < r.length; n++)
                    for (i = 0; i < o._tags.length; i++) o._valueComparer(o._tags[i], r[n]) && o._uncheckItemByUid(o._tags[i].uid)
            },
            _getNewValues: function(e, t) {
                var n, i = [];
                for (n = 0; n < e.length; n++) t.indexOf(e[n]) === -1 && i.push(e[n]);
                return i
            },
            _clearValue: function() {
                var e, t, n = this._dropdowntree,
                    i = n._tags.slice();
                for (e = 0; e < i.length; e++) t = i[e].uid, n._preventChangeTrigger = !0, n._uncheckItemByUid(t);
                i.length && (n._preventChangeTrigger = !1, n._valueMethodCalled || n.trigger(R))
            },
            _checkLoadedItem: function(e, n) {
                var i = this._dropdowntree;
                return i._noInitialValue && e.checked ? (i._checkValue(e), t) : (n.length && (n.indexOf(i._currentValue(e)) !== -1 || n.indexOf(e)) !== -1 && !this._findTag(i._currentValue(e)) && (e.checked ? i._checkValue(e) : e.set("checked", !0)), t)
            },
            _findTag: function(e) {
                var t = this._dropdowntree;
                return t._tags.find(function(n) {
                    return t._valueComparer(n, e)
                })
            }
        }), r.ui.DropDownTree.SingleSelection = i, r.ui.DropDownTree.MultipleSelection = o
    }(window.kendo.jQuery), window.kendo
}, "function" == typeof define && define.amd ? define : function(e, t, n) {
    (n || t)()
        }),
function(e, define) {
    define("kendo.listview.min", ["kendo.data.min", "kendo.editable.min", "kendo.selectable.min"], e)
}(function() {
    return function(e, t) {
        var n = window.kendo,
            i = "change",
            o = "cancel",
            r = "dataBound",
            s = "dataBinding",
            a = n.ui.Widget,
            l = n.keys,
            c = ">*:not(.k-loading-mask)",
            d = "progress",
            u = "error",
            h = "k-state-focused",
            p = "k-state-selected",
            f = "k-edit-item",
            m = "edit",
            g = "remove",
            v = "save",
            _ = "click",
            b = ".kendoListView",
            w = e.proxy,
            y = n._activeElement,
            k = n.ui.progress,
            x = n.data.DataSource,
            C = n.ui.DataBoundWidget.extend({
                init: function(t, i) {
                    var o = this;
                    i = e.isArray(i) ? {
                        dataSource: i
                    } : i, a.fn.init.call(o, t, i), i = o.options, o.wrapper = t = o.element, t[0].id && (o._itemId = t[0].id + "_lv_active"), o._element(), o._dataSource(), o._templates(), o._navigatable(), o._selectable(), o._pageable(), o._crudHandlers(), o.options.autoBind && o.dataSource.fetch(), n.notify(o)
                },
                events: [i, o, s, r, m, g, v],
                options: {
                    name: "ListView",
                    autoBind: !0,
                    selectable: !1,
                    navigatable: !1,
                    template: "",
                    altTemplate: "",
                    editTemplate: ""
                },
                setOptions: function(e) {
                    a.fn.setOptions.call(this, e), this._templates(), this.selectable && (this.selectable.destroy(), this.selectable = null), this._selectable()
                },
                _templates: function() {
                    var e = this.options;
                    this.template = n.template(e.template || ""), this.altTemplate = n.template(e.altTemplate || e.template), this.editTemplate = n.template(e.editTemplate || "")
                },
                _item: function(e) {
                    return this.element.children()[e]()
                },
                items: function() {
                    return this.element.children()
                },
                dataItem: function(t) {
                    var i = n.attr("uid"),
                        o = e(t).closest("[" + i + "]").attr(i);
                    return this.dataSource.getByUid(o)
                },
                setDataSource: function(e) {
                    this.options.dataSource = e, this._dataSource(), this.options.autoBind && e.fetch()
                },
                _unbindDataSource: function() {
                    var e = this;
                    e.dataSource.unbind(i, e._refreshHandler).unbind(d, e._progressHandler).unbind(u, e._errorHandler)
                },
                _dataSource: function() {
                    var e = this;
                    e.dataSource && e._refreshHandler ? e._unbindDataSource() : (e._refreshHandler = w(e.refresh, e), e._progressHandler = w(e._progress, e), e._errorHandler = w(e._error, e)), e.dataSource = x.create(e.options.dataSource).bind(i, e._refreshHandler).bind(d, e._progressHandler).bind(u, e._errorHandler)
                },
                _progress: function() {
                    k(this.element, !0)
                },
                _error: function() {
                    k(this.element, !1)
                },
                _element: function() {
                    this.element.addClass("k-widget k-listview").attr("role", "listbox")
                },
                refresh: function(e) {
                    var i, o, a, l, c, d = this,
                        u = d.dataSource.view(),
                        h = "",
                        p = d.template,
                        f = d.altTemplate,
                        m = y();
                    if (e = e || {}, "itemchange" === e.action) return d._hasBindingTarget() || d.editable || (i = e.items[0], a = d.items().filter("[" + n.attr("uid") + "=" + i.uid + "]"), a.length > 0 && (l = a.index(), d.angular("cleanup", function() {
                        return {
                            elements: [a]
                        }
                    }), a.replaceWith(p(i)), a = d.items().eq(l), a.attr(n.attr("uid"), i.uid), d.angular("compile", function() {
                        return {
                            elements: [a],
                            data: [{
                                dataItem: i
                            }]
                        }
                    }), d.trigger("itemChange", {
                        item: a,
                        data: i
                    }))), t;
                    if (!d.trigger(s, {
                            action: e.action || "rebind",
                            items: e.items,
                            index: e.index
                        })) {
                        for (d._angularItems("cleanup"), d._destroyEditable(), l = 0, c = u.length; l < c; l++) h += l % 2 ? f(u[l]) : p(u[l]);
                        for (d.element.html(h), o = d.items(), l = 0, c = u.length; l < c; l++) o.eq(l).attr(n.attr("uid"), u[l].uid).attr("role", "option").attr("aria-selected", "false");
                        d.element[0] === m && d.options.navigatable && d.current(o.eq(0)), d._angularItems("compile"), d.trigger(r, {
                            action: e.action || "rebind",
                            items: e.items,
                            index: e.index
                        })
                    }
                },
                _pageable: function() {
                    var t, i, o = this,
                        r = o.options.pageable;
                    e.isPlainObject(r) && (i = r.pagerId, t = e.extend({}, r, {
                        dataSource: o.dataSource,
                        pagerId: null
                    }), o.pager = new n.ui.Pager(e("#" + i), t))
                },
                _selectable: function() {
                    var e, o, r = this,
                        s = r.options.selectable,
                        a = r.options.navigatable;
                    s && (e = n.ui.Selectable.parseOptions(s).multiple, r.selectable = new n.ui.Selectable(r.element, {
                        aria: !0,
                        multiple: e,
                        filter: c,
                        change: function() {
                            r.trigger(i)
                        }
                    }), a && r.element.on("keydown" + b, function(n) {
                        if (n.keyCode === l.SPACEBAR) {
                            if (o = r.current(), n.target == n.currentTarget && n.preventDefault(), e)
                                if (n.ctrlKey) {
                                    if (o && o.hasClass(p)) return o.removeClass(p), t
                                } else r.selectable.clear();
                            else r.selectable.clear();
                            r.selectable.value(o)
                        }
                    }))
                },
                current: function(e) {
                    var n = this,
                        i = n.element,
                        o = n._current,
                        r = n._itemId;
                    return e === t ? o : (o && o[0] && (o[0].id === r && o.removeAttr("id"), o.removeClass(h), i.removeAttr("aria-activedescendant")), e && e[0] && (r = e[0].id || r, n._scrollTo(e[0]), i.attr("aria-activedescendant", r), e.addClass(h).attr("id", r)), n._current = e, t)
                },
                _scrollTo: function(t) {
                    var n, i, o = this,
                        r = !1,
                        s = "scroll";
                    "auto" == o.wrapper.css("overflow") || o.wrapper.css("overflow") == s ? n = o.wrapper[0] : (n = window, r = !0), i = function(i, o) {
                        var a = r ? e(t).offset()[i.toLowerCase()] : t["offset" + i],
                            l = t["client" + o],
                            c = e(n)[s + i](),
                            d = e(n)[o.toLowerCase()]();
                        a + l > c + d ? e(n)[s + i](a + l - d) : a < c && e(n)[s + i](a)
                    }, i("Top", "Height"), i("Left", "Width")
                },
                _navigatable: function() {
                    var t = this,
                        i = t.options.navigatable,
                        o = t.element,
                        r = function(i) {
                            t.current(e(i.currentTarget)), e(i.target).is(":button,a,:input,a>.k-icon,textarea") || n.focusElement(o)
                        };
                    i && (t._tabindex(), o.on("focus" + b, function() {
                        var e = t._current;
                        e && e.is(":visible") || (e = t._item("first")), t.current(e)
                    }).on("focusout" + b, function() {
                        t._current && t._current.removeClass(h)
                    }).on("keydown" + b, function(i) {
                        var r, s, a = i.keyCode,
                            c = t.current(),
                            d = e(i.target),
                            u = !d.is(":button,textarea,a,a>.t-icon,input"),
                            h = d.is(":text,:password"),
                            p = n.preventDefault,
                            m = o.find("." + f),
                            g = y();
                        if (!(!u && !h && l.ESC != a || h && l.ESC != a && l.ENTER != a))
                            if (l.UP === a || l.LEFT === a) c && (c = c.prev()), t.current(c && c[0] ? c : t._item("last")), p(i);
                            else if (l.DOWN === a || l.RIGHT === a) c && (c = c.next()), t.current(c && c[0] ? c : t._item("first")), p(i);
                        else if (l.PAGEUP === a) t.current(null), t.dataSource.page(t.dataSource.page() - 1), p(i);
                        else if (l.PAGEDOWN === a) t.current(null), t.dataSource.page(t.dataSource.page() + 1), p(i);
                        else if (l.HOME === a) t.current(t._item("first")), p(i);
                        else if (l.END === a) t.current(t._item("last")), p(i);
                        else if (l.ENTER === a) 0 !== m.length && (u || h) ? (r = t.items().index(m), g && g.blur(), t.save(), s = function() {
                            t.element.trigger("focus"),
                                t.current(t.items().eq(r))
                        }, t.one("dataBound", s)) : "" !== t.options.editTemplate && t.edit(c);
                        else if (l.ESC === a) {
                            if (m = o.find("." + f), 0 === m.length) return;
                            r = t.items().index(m), t.cancel(), t.element.trigger("focus"), t.current(t.items().eq(r))
                        }
                    }), o.on("mousedown" + b + " touchstart" + b, c, w(r, t)))
                },
                clearSelection: function() {
                    var e = this;
                    e.selectable.clear(), e.trigger(i)
                },
                select: function(n) {
                    var i = this,
                        o = i.selectable;
                    return n = e(n), n.length ? (o.options.multiple || (o.clear(), n = n.first()), o.value(n), t) : o.value()
                },
                _destroyEditable: function() {
                    var e = this;
                    e.editable && (e.editable.destroy(), delete e.editable)
                },
                _modelFromElement: function(e) {
                    var t = e.attr(n.attr("uid"));
                    return this.dataSource.getByUid(t)
                },
                _closeEditable: function() {
                    var e, t, i, o = this,
                        r = o.editable,
                        s = o.template;
                    return r && (r.element.index() % 2 && (s = o.altTemplate), o.angular("cleanup", function() {
                        return {
                            elements: [r.element]
                        }
                    }), e = o._modelFromElement(r.element), o._destroyEditable(), i = r.element.index(), r.element.replaceWith(s(e)), t = o.items().eq(i), t.attr(n.attr("uid"), e.uid), o._hasBindingTarget() && n.bind(t, e), o.angular("compile", function() {
                        return {
                            elements: [t],
                            data: [{
                                dataItem: e
                            }]
                        }
                    })), !0
                },
                edit: function(e) {
                    var t, i, o = this,
                        r = o._modelFromElement(e),
                        s = r.uid;
                    o.cancel(), e = o.items().filter("[" + n.attr("uid") + "=" + s + "]"), i = e.index(), e.replaceWith(o.editTemplate(r)), t = o.items().eq(i).addClass(f).attr(n.attr("uid"), r.uid), o.editable = t.kendoEditable({
                        model: r,
                        clearContainer: !1,
                        errorTemplate: !1,
                        target: o
                    }).data("kendoEditable"), o.trigger(m, {
                        model: r,
                        item: t
                    })
                },
                save: function() {
                    var e, t, n = this,
                        i = n.editable;
                    i && (t = i.element, e = n._modelFromElement(t), i.end() && !n.trigger(v, {
                        model: e,
                        item: t
                    }) && (n._closeEditable(), n.dataSource.sync()))
                },
                remove: function(e) {
                    var t = this,
                        n = t.dataSource,
                        i = t._modelFromElement(e);
                    t.editable && (n.cancelChanges(t._modelFromElement(t.editable.element)), t._closeEditable()), t.trigger(g, {
                        model: i,
                        item: e
                    }) || (e.hide(), n.remove(i), n.sync())
                },
                add: function() {
                    var e, t = this,
                        n = t.dataSource,
                        i = n.indexOf((n.view() || [])[0]);
                    i < 0 && (i = 0), t.cancel(), e = n.insert(i, {}), t.edit(t.element.find("[data-uid='" + e.uid + "']"))
                },
                cancel: function() {
                    var e, t, n = this,
                        i = n.dataSource;
                    n.editable && (e = n.editable.element, t = n._modelFromElement(e), n.trigger(o, {
                        model: t,
                        container: e
                    }) || (i.cancelChanges(t), n._closeEditable()))
                },
                _crudHandlers: function() {
                    var t = this,
                        i = _ + b;
                    t.element.on(i, ".k-edit-button", function(i) {
                        var o = e(this).closest("[" + n.attr("uid") + "]");
                        t.edit(o), i.preventDefault()
                    }), t.element.on(i, ".k-delete-button", function(i) {
                        var o = e(this).closest("[" + n.attr("uid") + "]");
                        t.remove(o), i.preventDefault()
                    }), t.element.on(i, ".k-update-button", function(e) {
                        t.save(), e.preventDefault()
                    }), t.element.on(i, ".k-cancel-button", function(e) {
                        t.cancel(), e.preventDefault()
                    })
                },
                destroy: function() {
                    var e = this;
                    a.fn.destroy.call(e), e._unbindDataSource(), e._destroyEditable(), e.element.off(b), e.pager && e.pager.destroy(), n.destroy(e.element)
                }
            });
        n.ui.plugin(C)
    }(window.kendo.jQuery), window.kendo
}, "function" == typeof define && define.amd ? define : function(e, t, n) {
    (n || t)()
}),
function(e, define) {
    define("kendo.listbox.min", ["kendo.draganddrop.min", "kendo.data.min", "kendo.selectable.min"], e)
}(function() {
    return function(e, t) {
        function n(t) {
            var n = e.map(t, function(t) {
                return e(t).index()
            });
            return n
        }

        function i(e) {
            return t === e
        }

        function o(e) {
            return e.clone().removeClass(ue).removeClass(U).addClass(_.format("{0} {1} {2}", H, K, q)).width(e.width())
        }

        function r() {
            return e("<li>").addClass(j)
        }
        var s, a, l, c, d, u, h, p, f, m, g, v, _ = window.kendo,
            b = _.attr,
            w = _.data,
            y = _.keys,
            k = _.template,
            x = _.ui.Widget,
            C = w.DataSource,
            S = _.ui.Selectable,
            T = _.ui.DataBoundWidget,
            D = _.Class,
            A = e.extend,
            E = e.noop,
            I = e.proxy,
            M = "-",
            R = ".",
            F = " ",
            P = "#",
            z = "kendoListBox",
            B = R + z,
            L = "k-state-disabled",
            H = "k-state-selected",
            N = ".k-item:not(.k-state-disabled)",
            O = ".k-list:not(.k-state-disabled) >" + N,
            V = "k-listbox-toolbar",
            W = "li > a.k-button:not(.k-state-disabled)",
            U = "k-state-focused",
            q = "k-drag-clue",
            j = "k-drop-hint",
            G = "k-reset k-list",
            $ = ".k-reset.k-list",
            K = "k-reset",
            Y = "click" + B,
            Q = "keydown" + B,
            X = "blur" + B,
            Z = _._outerWidth,
            J = _._outerHeight,
            ee = "change",
            te = "dataBound",
            ne = "add",
            ie = "remove",
            oe = "reorder",
            re = "moveUp",
            se = "moveDown",
            ae = "transferTo",
            le = "transferFrom",
            ce = "transferAllTo",
            de = "transferAllFrom",
            ue = "k-ghost",
            he = "uid",
            pe = "tabindex",
            fe = "command",
            me = -1,
            ge = 1,
            ve = "dragstart",
            _e = "drag",
            be = "drop",
            we = "dragend",
            ye = "ul.k-reset.k-list>li.k-item",
            ke = "right",
            xe = "bottom",
            Ce = [V + M + "left", V + M + ke, V + M + "top", V + M + xe],
            Se = T.extend({
                init: function(e, t) {
                    var n = this;
                    x.fn.init.call(n, e, t), n._wrapper(), n._list(), e = n.element.attr("multiple", "multiple").hide(), e[0] && !n.options.dataSource && (n.options.dataTextField = n.options.dataTextField || "text", n.options.dataValueField = n.options.dataValueField || "value"), n._templates(), n._selectable(), n._dataSource(), n._createToolbar(), n._createDraggable(), n._createNavigatable()
                },
                destroy: function() {
                    var e = this;
                    T.fn.destroy.call(e), isNaN(e._listTabIndex) || (e._getList().off(), e._listTabIndex = null), e._unbindDataSource(), e._destroySelectable(), e._destroyToolbar(), e.wrapper.off(B), e._target && (e._target = null), e._draggable && (e._draggable.destroy(), e.placeholder = null), _.destroy(e.element)
                },
                setOptions: function(e) {
                    x.fn.setOptions.call(this, e), this._templates(), this._dataSource()
                },
                events: [ee, te, ne, ie, oe, ve, _e, be, we],
                options: {
                    name: "ListBox",
                    autoBind: !0,
                    template: "",
                    dataTextField: "",
                    dataValueField: "",
                    selectable: "single",
                    draggable: null,
                    dropSources: [],
                    connectWith: "",
                    navigatable: !0,
                    toolbar: {
                        position: ke,
                        tools: []
                    },
                    messages: {
                        tools: {
                            remove: "Delete",
                            moveUp: "Move Up",
                            moveDown: "Move Down",
                            transferTo: "Transfer To",
                            transferFrom: "Transfer From",
                            transferAllTo: "Transfer All To",
                            transferAllFrom: "Transfer All From"
                        }
                    }
                },
                add: function(e) {
                    var t, n = this,
                        i = e && e.length ? e : [e],
                        o = i.length;
                    for (n._unbindDataSource(), t = 0; t < o; t++) n._addItem(i[t]);
                    n._bindDataSource(), n._syncElement()
                },
                _addItem: function(t) {
                    var n = this,
                        i = n.templates.itemTemplate({
                            item: t,
                            r: n.templates.itemContent
                        });
                    e(i).attr(b(he), t.uid).appendTo(n._getList()), "string" == typeof t ? n.dataSource._data.push(t) : n.dataSource.add(t)
                },
                _addItemAt: function(t, n) {
                    var i = this,
                        o = i.templates.itemTemplate({
                            item: t,
                            r: i.templates.itemContent
                        });
                    i._unbindDataSource(), "string" == typeof t ? (i._insertElementAt(o, n), i.dataSource._data.push(t)) : (i._insertElementAt(e(o).attr(b(he), t.uid), n), i.dataSource.add(t)), i._bindDataSource(), i._syncElement()
                },
                _insertElementAt: function(t, n) {
                    var i = this,
                        o = i._getList();
                    n > 0 ? e(t).insertAfter(o.children().eq(n - 1)) : e(o).prepend(t)
                },
                _createNavigatable: function() {
                    var e = this,
                        t = e.options;
                    t.navigatable && e._getList().on(Y, N, I(e._click, e)).on(Q, I(e._keyDown, e)).on(X, I(e._blur, e))
                },
                _getTabIndex: function() {
                    var e, t = this;
                    return isNaN(t._listTabIndex) ? (e = t.element.attr(pe), t._listTabIndex = isNaN(e) ? 0 : e, t.element.removeAttr(pe), t._listTabIndex) : t._listTabIndex
                },
                _blur: function() {
                    this._target && (this._target.removeClass(U), this._getList().removeAttr("aria-activedescendant")), this._target = null
                },
                _click: function(t) {
                    var n = this,
                        i = e(t.currentTarget),
                        o = n._target;
                    o && o.removeClass(U), n._target = i, i.addClass(U), n._getList().attr("aria-activedescendant", i.attr("id")), n._getList()[0] !== _._activeElement() && n.focus()
                },
                _getNavigatableItem: function(e) {
                    var t, n = this;
                    return t = n._target ? n._target : n.items().filter(N).first(), e === y.UP && n._target && (t = n._target.prevAll(N).first()), e === y.DOWN && n._target && (t = n._target.nextAll(N).first()), t.length ? t : null
                },
                _scrollIntoView: function(e) {
                    var t, n, i, o, r;
                    e && (e[0] && (e = e[0]), t = this._getList().parent()[0], n = e.offsetTop, i = t.scrollTop, o = t.clientHeight, r = n + e.offsetHeight, i > n ? i = n : r > i + o && (i = r - o), t.scrollTop = i)
                },
                _keyDown: function(e) {
                    var n, i = this,
                        o = e.keyCode,
                        r = i._getNavigatableItem(o);
                    if (i._target && i._target.removeClass(U), (!e.shiftKey || e.ctrlKey || o !== y.DOWN && o !== y.UP) && (i._shiftSelecting = !1), o == y.DELETE) i._executeCommand(ie), i._target && (i._target.removeClass(U), i._getList().removeAttr("aria-activedescendant"), i._target = null), n = !0;
                    else if (o === y.DOWN || o === y.UP) {
                        if (!r) return e.preventDefault(), t;
                        if (e.shiftKey && !e.ctrlKey) i._shiftSelecting || (i.clearSelection(), i._shiftSelecting = !0), i._target && r.hasClass("k-state-selected") ? (i._target.removeClass(H), i.trigger(ee)) : i.select("single" == i.options.selectable ? r : r.add(i._target));
                        else {
                            if (e.shiftKey && e.ctrlKey) return i._executeCommand(o === y.DOWN ? se : re), i._scrollIntoView(i._target), e.preventDefault(), t;
                            e.shiftKey || e.ctrlKey || ("multiple" === i.options.selectable && i.clearSelection(), i.select(r))
                        }
                        i._target = r, i._target ? (i._target.addClass(U), i._scrollIntoView(i._target), i._getList().attr("aria-activedescendant", i._target.attr("id"))) : i._getList().removeAttr("aria-activedescendant"), n = !0
                    } else o == y.SPACEBAR ? (e.ctrlKey && i._target ? i._target.hasClass(H) ? (i._target.removeClass(H), i.trigger(ee)) : i.select(i._target) : (i.clearSelection(), i.select(i._target)), n = !0) : e.ctrlKey && o == y.RIGHT ? (i._executeCommand(e.shiftKey ? ce : ae), i._target = i.select().length ? i.select() : null, n = !0) : e.ctrlKey && o == y.LEFT && (i._executeCommand(e.shiftKey ? de : le), n = !0);
                    n && e.preventDefault()
                },
                focus: function() {
                    _.focusElement(this._getList())
                },
                _createDraggable: function() {
                    var t, n = this,
                        i = n.options.draggable;
                    if (i) {
                        if (t = i.hint, !n.options.selectable) throw Error("Dragging requires selection to be enabled");
                        t || (t = o), n._draggable = new _.ui.Draggable(n.wrapper, {
                            filter: i.filter ? i.filter : ye,
                            hint: _.isFunction(t) ? t : e(t),
                            dragstart: I(n._dragstart, n),
                            dragcancel: I(n._clear, n),
                            drag: I(n._drag, n),
                            dragend: I(n._dragend, n)
                        })
                    }
                },
                _dragstart: function(n) {
                    var i = this,
                        o = i.draggedElement = n.currentTarget,
                        s = i.options.draggable.placeholder,
                        a = i.dataItem(o),
                        l = {
                            dataItems: a,
                            items: e(o),
                            draggableEvent: n
                        };
                    return i.options.draggable.enabled === !1 ? (n.preventDefault(), t) : (s || (s = r), i.placeholder = e(_.isFunction(s) ? s.call(i, o) : s), o.is(R + L) ? n.preventDefault() : i.trigger(ve, l) ? n.preventDefault() : (i.clearSelection(), i.select(o), o.addClass(ue)), t)
                },
                _clear: function() {
                    this.draggedElement.removeClass(ue), this.placeholder.remove()
                },
                _findElementUnderCursor: function(t) {
                    var n = _.elementUnderCursor(t),
                        i = t.sender;
                    return (e.contains(i.hint[0], n) || i.hint[0] === n) && (i.hint.hide(), n = _.elementUnderCursor(t), i.hint.show()), n
                },
                _findTarget: function(t) {
                    var n, i, o = this,
                        r = o._findElementUnderCursor(t),
                        s = e(r),
                        a = o._getList();
                    return e.contains(a[0], r) ? (n = o.items(), r = s.is("li") ? r : s.closest("li")[0], i = n.filter(r)[0] || n.has(r)[0], i ? (i = e(i), i.hasClass(L) ? null : {
                        element: i,
                        listBox: o
                    }) : null) : a[0] == r || a.parent()[0] == r ? {
                        element: e(a),
                        appendToBottom: !0,
                        listBox: o
                    } : o._searchConnectedListBox(s)
                },
                _getElementCenter: function(e) {
                    var t = e.length ? _.getOffset(e) : null;
                    return t && (t.top += J(e) / 2, t.left += Z(e) / 2), t
                },
                _searchConnectedListBox: function(t) {
                    var n, i, o, r, s = t;
                    return r = t.hasClass("k-list-scroller k-selectable") ? t : t.closest(".k-list-scroller.k-selectable"), r.length ? (n = r.parent().find("[data-role='listbox']").getKendoListBox(), n && e.inArray(this.element[0].id, n.options.dropSources) !== -1 ? (i = n.items(), t = t.is("li") ? t[0] : t.closest("li")[0], o = i.filter(t)[0] || i.has(t)[0], o ? (o = e(o), o.hasClass(L) ? null : {
                        element: o,
                        listBox: n
                    }) : !i.length || s.hasClass("k-list-scroller k-selectable") || s.hasClass("k-reset k-list") ? {
                        element: n._getList(),
                        listBox: n,
                        appendToBottom: !0
                    } : null) : null) : null
                },
                _drag: function(n) {
                    var i, o, r, s = this,
                        a = s.draggedElement,
                        l = s._findTarget(n),
                        c = {
                            left: n.x.location,
                            top: n.y.location
                        },
                        d = s.dataItem(a),
                        u = {
                            dataItems: [d],
                            items: e(a),
                            draggableEvent: n
                        };
                    if (s.trigger(_e, u)) return n.preventDefault(), t;
                    if (l) {
                        if (i = this._getElementCenter(l.element), o = {
                                left: Math.round(c.left - i.left),
                                top: Math.round(c.top - i.top)
                            }, l.appendToBottom) return s._movePlaceholder(l, null, a), t;
                        o.top < 0 ? r = "prev" : o.top > 0 && (r = "next"), r && l.element[0] != s.placeholder[0] && s._movePlaceholder(l, r, a)
                    } else s.placeholder.parent().length && s.placeholder.remove()
                },
                _movePlaceholder: function(t, n, i) {
                    var o = this,
                        s = o.placeholder,
                        a = t.listBox.options.draggable;
                    s.parent().length && (o.placeholder.remove(), o.placeholder = e(a && a.placeholder ? _.isFunction(a.placeholder) ? a.placeholder.call(o, i) : a.placeholder : r.call(o, i))), n ? "prev" === n ? t.element.before(o.placeholder) : "next" === n && t.element.after(o.placeholder) : t.element.append(o.placeholder)
                },
                _dragend: function(n) {
                    var i = this,
                        o = i.draggedElement,
                        r = i.items(),
                        s = r.not(i.draggedElement).index(i.placeholder),
                        a = r.not(i.placeholder).index(i.draggedElement),
                        l = i.dataItem(o),
                        c = {
                            dataItems: [l],
                            items: e(o)
                        },
                        d = i.placeholder.closest(".k-widget.k-listbox").find("[data-role='listbox']").getKendoListBox();
                    return i.trigger(be, A({}, c, {
                        draggableEvent: n
                    })) ? (n.preventDefault(), this._clear(), t) : (s >= 0 ? s === a || i.trigger(oe, A({}, c, {
                        offset: s - a
                    })) || (o.removeClass(ue), i.reorder(o, s)) : d && (i.trigger(ie, c) || i.remove(e(o)), d.trigger(ne, c) || d._addItemAt(l, d.items().index(i.placeholder))), i._clear(), i._draggable.dropped = !0, i.trigger(we, A({}, c, {
                        draggableEvent: n
                    })), i._updateToolbar(), i._updateAllToolbars(), t)
                },
                reorder: function(t, n) {
                    var i = this,
                        o = i.dataSource,
                        r = i.dataItem(t),
                        s = o.at(n),
                        a = i.items()[n],
                        l = e(t);
                    r && a && s && (i._removeElement(l), i._insertElementAt(l, n), i._updateToolbar())
                },
                remove: function(t) {
                    var n, i = this,
                        o = i._getItems(t),
                        r = o.length;
                    for (i._unbindDataSource(), n = 0; n < r; n++) i._removeItem(e(o[n]));
                    i._bindDataSource(), i._syncElement(), i._updateToolbar(), i._updateAllToolbars()
                },
                _removeItem: function(e) {
                    var t, n, i = this,
                        o = i.dataSource,
                        r = i.dataItem(e);
                    if (r && o) {
                        if ("string" == typeof r) {
                            for (t = o._data, n = 0; n < t.length; n++)
                                if (r === t[n]) {
                                    t[n] = t[t.length - 1], t.pop();
                                    break
                                }
                        } else o.remove(r);
                        i._removeElement(e)
                    }
                },
                _removeElement: function(t) {
                    _.destroy(t), e(t).off().remove()
                },
                dataItem: function(t) {
                    var n = b(he),
                        i = e(t).attr(n) || e(t).closest("[" + n + "]").attr(n);
                    return i ? this.dataSource.getByUid(i) : e(t).html()
                },
                _dataItems: function(t) {
                    var n, i = [],
                        o = e(t),
                        r = o.length;
                    for (n = 0; n < r; n++) i.push(this.dataItem(o.eq(n)));
                    return i
                },
                items: function() {
                    var e = this._getList();
                    return e.children()
                },
                select: function(e) {
                    var t, n = this,
                        o = n.selectable;
                    return i(e) ? o.value() : (t = n.items().filter(e).filter(O), o.options.multiple || (o.clear(), t = t.first()), o.value(t))
                },
                clearSelection: function() {
                    var e = this,
                        t = e.selectable;
                    t && t.clear()
                },
                enable: function(t, n) {
                    var o, r = this,
                        s = !!i(n) || !!n,
                        a = r._getItems(t),
                        l = a.length;
                    for (o = 0; o < l; o++) r._enableItem(e(a[o]), s);
                    r._updateAllToolbars()
                },
                _enableItem: function(t, n) {
                    var i = this,
                        o = i.dataItem(t);
                    o && (n ? e(t).removeClass(L) : e(t).addClass(L).removeClass(H))
                },
                setDataSource: function(e) {
                    var t = this;
                    t.options.dataSource = e, t._dataSource()
                },
                _dataSource: function() {
                    var t = this,
                        n = t.options,
                        i = n.dataSource || {};
                    i = e.isArray(i) ? {
                        data: i
                    } : i, i.select = t.element, i.fields = [{
                        field: n.dataTextField
                    }, {
                        field: n.dataValueField
                    }], t._unbindDataSource(), t.dataSource = C.create(i), t._bindDataSource(), t.options.autoBind && t.dataSource.fetch()
                },
                _bindDataSource: function() {
                    var e = this,
                        t = e.dataSource;
                    e._dataChangeHandler = I(e.refresh, e), t && t.bind(ee, e._dataChangeHandler)
                },
                _unbindDataSource: function() {
                    var e = this,
                        t = e.dataSource;
                    t && t.unbind(ee, e._dataChangeHandler)
                },
                _wrapper: function() {
                    var t = this,
                        n = t.element,
                        i = n.parent("div.k-listbox");
                    i[0] || (i = n.wrap('<div class="k-widget k-listbox" deselectable="on" />').parent(), i[0].style.cssText = n[0].style.cssText, i[0].title = n[0].title, e('<div class="k-list-scroller" />').insertBefore(n)), t.wrapper = i.addClass(n[0].className).css("display", ""), t._innerWrapper = e(i[0].firstChild)
                },
                _list: function() {
                    var t = this;
                    e("<ul class='" + G + "' role='listbox'></ul>").appendTo(t._innerWrapper), t.options.navigatable && t._getList().attr(pe, t._getTabIndex())
                },
                _templates: function() {
                    var e, t = this,
                        n = this.options;
                    e = n.template && "string" == typeof n.template ? _.template(n.template) : n.template ? n.template : _.template("${" + _.expr(n.dataTextField, "data") + "}", {
                        useWithBlock: !1
                    }), t.templates = {
                        itemTemplate: _.template("# var item = data.item, r = data.r; # <li class='k-item' role='option' aria-selected='false'>#=r(item)#</li>", {
                            useWithBlock: !1
                        }),
                        itemContent: e,
                        toolbar: "<div class='" + V + "'></div>"
                    }
                },
                refresh: function() {
                    var e, t = this,
                        n = t.dataSource.view(),
                        i = t.templates.itemTemplate,
                        o = "";
                    for (e = 0; e < n.length; e++) o += i({
                        item: n[e],
                        r: t.templates.itemContent
                    });
                    t._getList().html(o), t._setItemIds(), t._createToolbar(), t._syncElement(), t._updateToolbar(), t._updateAllToolbars(), t.trigger(te)
                },
                _syncElement: function() {
                    var e, t = "",
                        n = this.dataSource.view();
                    for (e = 0; e < n.length; e++) t += this._option(n[e][this.options.dataValueField] || n[e], n[e][this.options.dataTextField] || n[e], !0);
                    this.element.html(t)
                },
                _option: function(e, n) {
                    var i = "<option";
                    return e !== t && (e += "", e.indexOf('"') !== -1 && (e = e.replace(/"/g, "&quot;")), i += ' value="' + e + '"'), i += " selected>", n !== t && (i += _.htmlEncode(n)), i += "</option>"
                },
                _setItemIds: function() {
                    var e, t = this,
                        n = t.items(),
                        i = t.dataSource.view(),
                        o = i.length;
                    for (e = 0; e < o; e++) n.eq(e).attr(b(he), i[e].uid).attr("id", i[e].uid)
                },
                _selectable: function() {
                    var e = this,
                        t = e.options.selectable,
                        n = S.parseOptions(t);
                    n.multiple && e.element.attr("aria-multiselectable", "true"), e.selectable = new S(e._innerWrapper, {
                        aria: !0,
                        multiple: n.multiple,
                        filter: N,
                        change: I(e._onSelect, e)
                    })
                },
                _onSelect: function() {
                    var e = this;
                    e._updateToolbar(), e._updateAllToolbars(), e.trigger(ee)
                },
                _destroySelectable: function() {
                    var e = this;
                    e.selectable && e.selectable.element && (e.selectable.destroy(), e.selectable = null)
                },
                _getList: function() {
                    return this.wrapper.find($)
                },
                _getItems: function(e) {
                    return this.items().filter(e)
                },
                _createToolbar: function() {
                    var t, n = this,
                        i = n.options.toolbar,
                        o = i.position || ke,
                        r = o === xe ? "insertAfter" : "insertBefore",
                        s = i.tools || [],
                        a = n.options.messages;
                    n._destroyToolbar(), n.wrapper.removeClass(Ce.join(F)), s.length && s.length > 0 && (t = e(n.templates.toolbar)[r](n._innerWrapper), n.toolbar = new v(t, A({}, i, {
                        listBox: n,
                        messages: a
                    })), n.wrapper.addClass(V + M + o))
                },
                _destroyToolbar: function() {
                    var e = this;
                    e.toolbar && (e.toolbar.destroy(), e.toolbar = null)
                },
                _executeCommand: function(e) {
                    var t = this,
                        n = s.current.create(e, {
                            listBox: t
                        });
                    n && (n.execute(), t._updateToolbar(), t._updateAllToolbars())
                },
                _updateToolbar: function() {
                    var e = this.toolbar;
                    e && e._updateToolStates()
                },
                _updateAllToolbars: function() {
                    var t, n, i = e("select[data-role='listbox']"),
                        o = i.length;
                    for (n = 0; n < o; n++) t = e(i[n]).data(z), t && t._updateToolbar()
                }
            });
        _.ui.plugin(Se), s = D.extend({
            init: function() {
                this._commands = []
            },
            register: function(e, t) {
                this._commands.push({
                    commandName: e,
                    commandType: t
                })
            },
            create: function(e, t) {
                var n, i, o, r = this._commands,
                    s = r.length,
                    a = e ? e.toLowerCase() : "";
                for (o = 0; o < s; o++)
                    if (i = r[o], i.commandName.toLowerCase() === a) {
                        n = i;
                        break
                    }
                if (n) return new n.commandType(t)
            }
        }), s.current = new s, a = D.extend({
            init: function(e) {
                var t = this;
                t.options = A({}, t.options, e), t.listBox = t.options.listBox
            },
            options: {
                listBox: null
            },
            getItems: function() {
                return e(this.listBox.select())
            },
            execute: E,
            canExecute: E
        }), l = a.extend({
            execute: function() {
                var e = this,
                    t = e.listBox,
                    n = e.getItems();
                t.trigger(ie, {
                    dataItems: t._dataItems(n),
                    items: n
                }) || t.remove(n)
            },
            canExecute: function() {
                return this.listBox.select().length > 0
            }
        }), s.current.register(ie, l), c = a.extend({
            execute: function() {
                var e = this;
                e.canExecute() && e.moveItems()
            },
            canExecute: E,
            moveItems: function() {
                var t, i = this,
                    o = i.listBox,
                    r = i.options,
                    s = i.getItems(),
                    a = r.offset,
                    l = n(s),
                    c = e.makeArray(s.sort(i.itemComparer)),
                    d = r.moveAction;
                if (!o.trigger(oe, {
                        dataItems: o._dataItems(c),
                        items: e(c),
                        offset: a
                    }))
                    for (; c.length > 0 && l.length > 0;) t = c[d](), o.reorder(t, l[d]() + a)
            },
            options: {
                offset: 0,
                moveAction: "pop"
            },
            itemComparer: function(t, n) {
                var i = e(t).index(),
                    o = e(n).index();
                return i === o ? 0 : i > o ? 1 : -1
            }
        }), d = c.extend({
            options: {
                offset: me,
                moveAction: "shift"
            },
            canExecute: function() {
                var e = this.getItems(),
                    t = n(e);
                return t.length > 0 && t[0] > 0
            }
        }), s.current.register(re, d), u = c.extend({
            options: {
                offset: ge,
                moveAction: "pop"
            },
            canExecute: function() {
                var t = this,
                    i = t.getItems(),
                    o = n(i);
                return o.length > 0 && e(o).last()[0] < t.listBox.items().length - 1
            }
        }), s.current.register(se, u), h = a.extend({
            options: {
                filter: N
            },
            execute: function() {
                var e = this,
                    t = e.getSourceListBox(),
                    n = e.getItems().filter(e.options.filter),
                    i = t ? t._dataItems(n) : [],
                    o = e.getDestinationListBox(),
                    r = e.getUpdatedSelection(n);
                o && n.length > 0 && (o.trigger(ne, {
                    dataItems: i,
                    items: n
                }) || o.add(i), t.trigger(ie, {
                    dataItems: i,
                    items: n
                }) || (t.remove(n), e.updateSelection(r)))
            },
            getUpdatedSelection: function(t) {
                var n = this,
                    i = n.options.filter,
                    o = n.getSourceListBox(),
                    r = o ? o.items().filter(i).last() : null,
                    s = e(t).filter(r).length > 0,
                    a = s ? e(t).prevAll(i)[0] : e(t).nextAll(i)[0];
                return 1 === e(t).length && a ? a : null
            },
            updateSelection: function(t) {
                var n = this.getSourceListBox();
                n && t && (e(n.select(e(t))), n._scrollIntoView(t))
            },
            getSourceListBox: E,
            getDestinationListBox: E
        }), p = h.extend({
            canExecute: function() {
                var e = this.getSourceListBox();
                return !!e && e.select().length > 0
            },
            getSourceListBox: function() {
                return this.listBox
            },
            getDestinationListBox: function() {
                var t = this.getSourceListBox();
                return t && t.options.connectWith ? e(P + t.options.connectWith).data(z) : null
            },
            getItems: function() {
                var t = this.getSourceListBox();
                return t ? e(t.select()) : e()
            }
        }), s.current.register(ae, p), f = h.extend({
            canExecute: function() {
                var e = this.getSourceListBox();
                return !!e && e.select().length > 0
            },
            getSourceListBox: function() {
                var t = this.getDestinationListBox();
                return t && t.options.connectWith ? e(P + t.options.connectWith).data(z) : null
            },
            getDestinationListBox: function() {
                return this.listBox
            },
            getItems: function() {
                var t = this.getSourceListBox();
                return t ? e(t.select()) : e()
            }
        }), s.current.register(le, f), m = p.extend({
            canExecute: function() {
                var e = this.getSourceListBox();
                return !!e && e.items().filter(N).length > 0
            },
            getItems: function() {
                var t = this.getSourceListBox();
                return t ? t.items() : e()
            },
            getUpdatedSelection: E,
            updateSelection: E
        }), s.current.register(ce, m), g = f.extend({
            canExecute: function() {
                var e = this.getSourceListBox();
                return !!e && e.items().filter(N).length > 0
            },
            getItems: function() {
                var t = this.getSourceListBox();
                return t ? t.items() : e()
            },
            getUpdatedSelection: E,
            updateSelection: E
        }), s.current.register(de, g), v = D.extend({
            init: function(t, n) {
                var i = this;
                i.element = e(t).addClass(V), i.options = A({}, i.options, n), i.listBox = i.options.listBox, i._initTemplates(), i._createTools(), i._updateToolStates(), i._attachEventHandlers()
            },
            destroy: function() {
                var e = this;
                e._detachEventHandlers(), _.destroy(e.element), e.element.remove(), e.element = null
            },
            options: {
                position: ke,
                tools: []
            },
            _initTemplates: function() {
                this.templates = {
                    tool: k("<li><a href='\\\\#' class='k-button k-button-icon' data-command='#= command #' title='#= text #' aria-label='#= text #' role='button'><span class='k-icon #= iconClass #'></span></a></li>")
                }
            },
            _createTools: function() {
                var t, n, i = this,
                    o = i.options.tools,
                    r = o.length,
                    s = i.options.messages.tools,
                    a = i._createToolList();
                for (n = 0; n < r; n++) t = A({}, v.defaultTools[o[n]], {
                    text: s[o[n]]
                }), t && a.append(e(i.templates.tool(t)));
                i.element.append(a)
            },
            _createToolList: function() {
                return e("<ul class='k-reset' />")
            },
            _attachEventHandlers: function() {
                var e = this;
                e.element.on(Y, W, I(e._onToolClick, e))
            },
            _detachEventHandlers: function() {
                this.element.off(B).find("*").off(B)
            },
            _onToolClick: function(t) {
                t.preventDefault(), this._executeToolCommand(e(t.currentTarget).data(fe))
            },
            _executeToolCommand: function(e) {
                var t = this,
                    n = t.listBox;
                n && n._executeCommand(e)
            },
            _updateToolStates: function() {
                var e, t = this,
                    n = t.options.tools,
                    i = n.length;
                for (e = 0; e < i; e++) t._updateToolState(n[e])
            },
            _updateToolState: function(t) {
                var n = this,
                    i = s.current.create(t, {
                        listBox: n.listBox
                    }),
                    o = n.element.find("[data-command='" + t + "']")[0];
                o && i && i.canExecute && (i.canExecute() ? e(o).removeClass(L).removeAttr(pe) : e(o).addClass(L).attr(pe, "-1"))
            }
        }), v.defaultTools = {
            remove: {
                command: ie,
                iconClass: "k-i-x"
            },
            moveUp: {
                command: re,
                iconClass: "k-i-arrow-60-up"
            },
            moveDown: {
                command: se,
                iconClass: "k-i-arrow-60-down"
            },
            transferTo: {
                command: ae,
                iconClass: "k-i-arrow-60-right"
            },
            transferFrom: {
                command: le,
                iconClass: "k-i-arrow-60-left"
            },
            transferAllTo: {
                command: ce,
                iconClass: "k-i-arrow-double-60-right"
            },
            transferAllFrom: {
                command: de,
                iconClass: "k-i-arrow-double-60-left"
            }
        }, A(Se, {
            ToolBar: v
        })
    }(window.kendo.jQuery), window.kendo
}, "function" == typeof define && define.amd ? define : function(e, t, n) {
    (n || t)()
}),
function(e, define) {
    define("kendo.web.min", ["kendo.data.min", "kendo.popup.min", "kendo.list.min", "kendo.dropdownlist.min", "kendo.dropdowntree.min","kendo.buttontree.min", "kendo.combobox.min", "kendo.multiselect.min", "kendo.multicolumncombobox.min", , "kendo.grid.min", "kendo.listview.min", "kendo.listbox.min", "kendo.editor.min", "kendo.editable.min", "kendo.treeview.min"], e)
}(function() {
    "bundle all";
    return window.kendo
}, "function" == typeof define && define.amd ? define : function(e, t, n) {
    (n || t)()
}),
function(e, define) {
    define("kendo.all.min", ["kendo.web.min"], e)
}(function() {
    "bundle all";
    return window.kendo
}, "function" == typeof define && define.amd ? define : function(e, t, n) {
    (n || t)()
});