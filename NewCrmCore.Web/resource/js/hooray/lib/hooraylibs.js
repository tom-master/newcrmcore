/**
 * 整理：胡尐睿丶
 * 联系：hooray@hoorayos.com
 */

/**
 * ie6 png透明修正
 * DD_belatedPNG.fix('.png_bg');
 * DD_belatedPNG.fixPng( someNode );
 * http://www.dillerdesign.com/experiment/DD_belatedPNG/
 */
if ($.browser.msie && $.browser.version === "6.0" && !$.support.style) {
	var DD_belatedPNG = {
		ns: "DD_belatedPNG",
		imgSize: {},
		delay: 10,
		nodesFixed: 0,
		createVmlNameSpace: function () {
			if (document.namespaces && !document.namespaces[this.ns]) {
				document.namespaces.add(this.ns, "urn:schemas-microsoft-com:vml");
			}
		},
		createVmlStyleSheet: function () {
			var b, a;
			b = document.createElement("style");
			b.setAttribute("media", "screen");
			document.documentElement.firstChild.insertBefore(b, document.documentElement.firstChild.firstChild);
			if (b.styleSheet) {
				b = b.styleSheet;
				b.addRule(this.ns + "\\:*", "{behavior:url(#default#VML)}");
				b.addRule(this.ns + "\\:shape", "position:absolute;");
				b.addRule("img." + this.ns + "_sizeFinder", "behavior:none; border:none; position:absolute; z-index:-1; top:-10000px; visibility:hidden;");
				this.screenStyleSheet = b;
				a = document.createElement("style");
				a.setAttribute("media", "print");
				document.documentElement.firstChild.insertBefore(a, document.documentElement.firstChild.firstChild);
				a = a.styleSheet;
				a.addRule(this.ns + "\\:*", "{display: none !important;}");
				a.addRule("img." + this.ns + "_sizeFinder", "{display: none !important;}");
			}
		},
		readPropertyChange: function () {
			var b, c, a;
			b = event.srcElement;
			if (!b.vmlInitiated) {
				return;
			}
			if (event.propertyName.search("background") !== -1 || event.propertyName.search("border") !== -1) {
				DD_belatedPNG.applyVML(b);
			}
			if (event.propertyName === "style.display") {
				c = b.currentStyle.display === "none" ? "none" : "block";
				for (a in b.vml) {
					if (b.vml.hasOwnProperty(a)) {
						b.vml[a].shape.style.display = c;
					}
				}
			}
			if (event.propertyName.search("filter") !== -1) {
				DD_belatedPNG.vmlOpacity(b);
			}
		},
		vmlOpacity: function (b) {
			if (b.currentStyle.filter.search("lpha") !== -1) {
				var a = b.currentStyle.filter;
				a = parseInt(a.substring(a.lastIndexOf("=") + 1, a.lastIndexOf(")")), 10) / 100;
				b.vml.color.shape.style.filter = b.currentStyle.filter;
				b.vml.image.fill.opacity = a;
			}
		},
		handlePseudoHover: function (a) {
			setTimeout(function () {
				DD_belatedPNG.applyVML(a);
			},
				1);
		},
		fix: function (a) {
			if (this.screenStyleSheet) {
				var c, b;
				c = a.split(",");
				for (b = 0; b < c.length; b++) {
					this.screenStyleSheet.addRule(c[b], "behavior:expression(DD_belatedPNG.fixPng(this))");
				}
			}
		},
		applyVML: function (a) {
			a.runtimeStyle.cssText = "";
			this.vmlFill(a);
			this.vmlOffsets(a);
			this.vmlOpacity(a);
			if (a.isImg) {
				this.copyImageBorders(a);
			}
		},
		attachHandlers: function (i) {
			var d, c, g, e, b, f;
			d = this;
			c = {
				resize: "vmlOffsets",
				move: "vmlOffsets"
			};
			if (i.nodeName === "A") {
				e = {
					mouseleave: "handlePseudoHover",
					mouseenter: "handlePseudoHover",
					focus: "handlePseudoHover",
					blur: "handlePseudoHover"
				};
				for (b in e) {
					if (e.hasOwnProperty(b)) {
						c[b] = e[b];
					}
				}
			}
			for (f in c) {
				if (c.hasOwnProperty(f)) {
					g = function () {
						d[c[f]](i);
					};
					i.attachEvent("on" + f, g);
				}
			}
			i.attachEvent("onpropertychange", this.readPropertyChange);
		},
		giveLayout: function (a) {
			a.style.zoom = 1;
			if (a.currentStyle.position === "static") {
				a.style.position = "relative";
			}
		},
		copyImageBorders: function (b) {
			var c, a;
			c = {
				borderStyle: true,
				borderWidth: true,
				borderColor: true
			};
			for (a in c) {
				if (c.hasOwnProperty(a)) {
					b.vml.color.shape.style[a] = b.currentStyle[a];
				}
			}
		},
		vmlFill: function (e) {
			if (!e.currentStyle) {
				return;
			} else {
				var d, f, g, b, a, c;
				d = e.currentStyle;
			}
			for (b in e.vml) {
				if (e.vml.hasOwnProperty(b)) {
					e.vml[b].shape.style.zIndex = d.zIndex;
				}
			}
			e.runtimeStyle.backgroundColor = "";
			e.runtimeStyle.backgroundImage = "";
			f = true;
			if (d.backgroundImage !== "none" || e.isImg) {
				if (!e.isImg) {
					e.vmlBg = d.backgroundImage;
					e.vmlBg = e.vmlBg.substr(5, e.vmlBg.lastIndexOf('")') - 5);
				} else {
					e.vmlBg = e.src;
				}
				g = this;
				if (!g.imgSize[e.vmlBg]) {
					a = document.createElement("img");
					g.imgSize[e.vmlBg] = a;
					a.className = g.ns + "_sizeFinder";
					a.runtimeStyle.cssText = "behavior:none; position:absolute; left:-10000px; top:-10000px; border:none; margin:0; padding:0;";
					c = function () {
						this.width = this.offsetWidth;
						this.height = this.offsetHeight;
						g.vmlOffsets(e);
					};
					a.attachEvent("onload", c);
					a.src = e.vmlBg;
					a.removeAttribute("width");
					a.removeAttribute("height");
					document.body.insertBefore(a, document.body.firstChild);
				}
				e.vml.image.fill.src = e.vmlBg;
				f = false;
			}
			e.vml.image.fill.on = !f;
			e.vml.image.fill.color = "none";
			e.vml.color.shape.style.backgroundColor = d.backgroundColor;
			e.runtimeStyle.backgroundImage = "none";
			e.runtimeStyle.backgroundColor = "transparent";
		},
		vmlOffsets: function (d) {
			var h, n, a, e, g, m, f, l, j, i, k;
			h = d.currentStyle;
			n = {
				W: d.clientWidth + 1,
				H: d.clientHeight + 1,
				w: this.imgSize[d.vmlBg].width,
				h: this.imgSize[d.vmlBg].height,
				L: d.offsetLeft,
				T: d.offsetTop,
				bLW: d.clientLeft,
				bTW: d.clientTop
			};
			a = n.L + n.bLW === 1 ? 1 : 0;
			e = function (b, p, q, c, s, u) {
				b.coordsize = c + "," + s;
				b.coordorigin = u + "," + u;
				b.path = "m0,0l" + c + ",0l" + c + "," + s + "l0," + s + " xe";
				b.style.width = c + "px";
				b.style.height = s + "px";
				b.style.left = p + "px";
				b.style.top = q + "px";
			};
			e(d.vml.color.shape, (n.L + (d.isImg ? 0 : n.bLW)), (n.T + (d.isImg ? 0 : n.bTW)), (n.W - 1), (n.H - 1), 0);
			e(d.vml.image.shape, (n.L + n.bLW), (n.T + n.bTW), (n.W), (n.H), 1);
			g = {
				X: 0,
				Y: 0
			};
			if (d.isImg) {
				g.X = parseInt(h.paddingLeft, 10) + 1;
				g.Y = parseInt(h.paddingTop, 10) + 1;
			} else {
				for (j in g) {
					if (g.hasOwnProperty(j)) {
						this.figurePercentage(g, n, j, h["backgroundPosition" + j]);
					}
				}
			}
			d.vml.image.fill.position = (g.X / n.W) + "," + (g.Y / n.H);
			m = h.backgroundRepeat;
			f = {
				T: 1,
				R: n.W + a,
				B: n.H,
				L: 1 + a
			};
			l = {
				X: {
					b1: "L",
					b2: "R",
					d: "W"
				},
				Y: {
					b1: "T",
					b2: "B",
					d: "H"
				}
			};
			if (m !== "repeat" || d.isImg) {
				i = {
					T: g.Y,
					R: g.X + n.w,
					B: g.Y + n.h,
					L: g.X
				};
				if (m.search("repeat-") !== -1) {
					k = m.split("repeat-")[1].toUpperCase();
					i[l[k].b1] = 1;
					i[l[k].b2] = n[l[k].d];
				}
				if (i.B > n.H) {
					i.B = n.H;
				}
				d.vml.image.shape.style.clip = "rect(" + i.T + "px " + (i.R + a) + "px " + i.B + "px " + (i.L + a) + "px)";
			} else {
				d.vml.image.shape.style.clip = "rect(" + f.T + "px " + f.R + "px " + f.B + "px " + f.L + "px)";
			}
		},
		figurePercentage: function (d, c, f, a) {
			var b, e;
			e = true;
			b = f === "X";
			switch (a) {
				case "left":
				case "top":
					d[f] = 0;
					break;
				case "center":
					d[f] = 0.5;
					break;
				case "right":
				case "bottom":
					d[f] = 1;
					break;
				default:
					if (a.search("%") !== -1) {
						d[f] = parseInt(a, 10) / 100;
					} else {
						e = false;
					}
			}
			d[f] = Math.ceil(e ? ((c[b ? "W" : "H"] * d[f]) - (c[b ? "w" : "h"] * d[f])) : parseInt(a, 10));
			if (d[f] % 2 === 0) {
				d[f]++;
			}
			return d[f];
		},
		fixPng: function (c) {
			c.style.behavior = "none";
			var g, b, f, a, d;
			if (c.nodeName === "BODY" || c.nodeName === "TD" || c.nodeName === "TR") {
				return;
			}
			c.isImg = false;
			if (c.nodeName === "IMG") {
				if (c.src.toLowerCase().search(/\.png$/) !== -1) {
					c.isImg = true;
					c.style.visibility = "hidden";
				} else {
					return;
				}
			} else {
				if (c.currentStyle.backgroundImage.toLowerCase().search(".png") === -1) {
					return;
				}
			}
			g = DD_belatedPNG;
			c.vml = {
				color: {},
				image: {}
			};
			b = {
				shape: {},
				fill: {}
			};
			for (a in c.vml) {
				if (c.vml.hasOwnProperty(a)) {
					for (d in b) {
						if (b.hasOwnProperty(d)) {
							f = g.ns + ":" + d;
							c.vml[a][d] = document.createElement(f);
						}
					}
					c.vml[a].shape.stroked = false;
					c.vml[a].shape.appendChild(c.vml[a].fill);
					c.parentNode.insertBefore(c.vml[a].shape, c);
				}
			}
			c.vml.image.shape.fillcolor = "none";
			c.vml.image.fill.type = "tile";
			c.vml.color.fill.on = false;
			g.attachHandlers(c);
			g.giveLayout(c);
			g.giveLayout(c.offsetParent);
			c.vmlInitiated = true;
			g.applyVML(c);
		}
	};
	try {
		document.execCommand("BackgroundImageCache", false, true);
	} catch (r) { Console.log(r); }
	DD_belatedPNG.createVmlNameSpace();
	DD_belatedPNG.createVmlStyleSheet();
}

/**
 * SWFObject v2.2
 * http://code.google.com/p/swfobject/
 * swfobject.embedSWF("test.swf", "myContent", "300", "120", "9.0.0", "expressInstall.swf");
 */
var swfobject = function () {
	var D = "undefined",
		r = "object",
		S = "Shockwave Flash",
		W = "ShockwaveFlash.ShockwaveFlash",
		q = "application/x-shockwave-flash",
		R = "SWFObjectExprInst",
		x = "onreadystatechange",
		O = window,
		j = document,
		t = navigator,
		T = false,
		U = [h],
		o = [],
		N = [],
		I = [],
		l,
		Q,
		E,
		B,
		J = false,
		a = false,
		n,
		G,
		m = true,
		M = function () {
			var aa = typeof j.getElementById !== D && typeof j.getElementsByTagName !== D && typeof j.createElement !== D,
				ah = t.userAgent.toLowerCase(),
				Y = t.platform.toLowerCase(),
				ae = Y ? /win/.test(Y) : /win/.test(ah),
				ac = Y ? /mac/.test(Y) : /mac/.test(ah),
				af = /webkit/.test(ah) ? parseFloat(ah.replace(/^.*webkit\/(\d+(\.\d+)?).*$/, "$1")) : false,
				X = !+"\v1",
				ag = [0, 0, 0],
				ab = null;
			if (typeof t.plugins !== D && typeof t.plugins[S] === r) {
				ab = t.plugins[S].description;
				if (ab && !(typeof t.mimeTypes !== D && t.mimeTypes[q] && !t.mimeTypes[q].enabledPlugin)) {
					T = true;
					X = false;
					ab = ab.replace(/^.*\s+(\S+\s+\S+$)/, "$1");
					ag[0] = parseInt(ab.replace(/^(.*)\..*$/, "$1"), 10);
					ag[1] = parseInt(ab.replace(/^.*\.(.*)\s.*$/, "$1"), 10);
					ag[2] = /[a-zA-Z]/.test(ab) ? parseInt(ab.replace(/^.*[a-zA-Z]+(.*)$/, "$1"), 10) : 0;
				}
			} else {
				if (typeof O.ActiveXObject !== D) {
					try {
						var ad = new ActiveXObject(W);
						if (ad) {
							ab = ad.GetVariable("$version");
							if (ab) {
								X = true;
								ab = ab.split(" ")[1].split(",");
								ag = [parseInt(ab[0], 10), parseInt(ab[1], 10), parseInt(ab[2], 10)];
							}
						}
					} catch (Z) { console.log(Z); }
				}
			}
			return {
				w3: aa,
				pv: ag,
				wk: af,
				ie: X,
				win: ae,
				mac: ac
			};
		}(),
		k = function () {
			if (!M.w3) {
				return;
			}
			if ((typeof j.readyState !== D && j.readyState === "complete") || (typeof j.readyState === D && (j.getElementsByTagName("body")[0] || j.body))) {
				f();
			}
			if (!J) {
				if (typeof j.addEventListener !== D) {
					j.addEventListener("DOMContentLoaded", f, false);
				}
				if (M.ie && M.win) {
					j.attachEvent(x,
						function () {
							if (j.readyState === "complete") {
								j.detachEvent(x, arguments.callee);
								f();
							}
						});
					if (O === top) {
						(function () {
							if (J) {
								return;
							}
							try {
								j.documentElement.doScroll("left");
							} catch (X) {
								setTimeout(arguments.callee, 0);
								return;
							}
							f();
						})();
					}
				}
				if (M.wk) {
					(function () {
						if (J) {
							return;
						}
						if (!/loaded|complete/.test(j.readyState)) {
							setTimeout(arguments.callee, 0);
							return;
						}
						f();
					})();
				}
				s(f);
			}
		}();
	function f() {
		if (J) {
			return;
		}
		try {
			var Z = j.getElementsByTagName("body")[0].appendChild(C("span"));
			Z.parentNode.removeChild(Z);
		} catch (aa) {
			return;
		}
		J = true;
		var X = U.length;
		for (var Y = 0; Y < X; Y++) {
			U[Y]();
		}
	}
	function K(X) {
		if (J) {
			X();
		} else {
			U[U.length] = X;
		}
	}
	function s(Y) {
		if (typeof O.addEventListener !== D) {
			O.addEventListener("load", Y, false);
		} else {
			if (typeof j.addEventListener !== D) {
				j.addEventListener("load", Y, false);
			} else {
				if (typeof O.attachEvent !== D) {
					i(O, "onload", Y);
				} else {
					if (typeof O.onload === "function") {
						var X = O.onload;
						O.onload = function () {
							X();
							Y();
						};
					} else {
						O.onload = Y;
					}
				}
			}
		}
	}
	function h() {
		if (T) {
			V();
		} else {
			H();
		}
	}
	function V() {
		var X = j.getElementsByTagName("body")[0];
		var aa = C(r);
		aa.setAttribute("type", q);
		var Z = X.appendChild(aa);
		if (Z) {
			var Y = 0; (function () {
				if (typeof Z.GetVariable !== D) {
					var ab = Z.GetVariable("$version");
					if (ab) {
						ab = ab.split(" ")[1].split(",");
						M.pv = [parseInt(ab[0], 10), parseInt(ab[1], 10), parseInt(ab[2], 10)];
					}
				} else {
					if (Y < 10) {
						Y++;
						setTimeout(arguments.callee, 10);
						return;
					}
				}
				X.removeChild(aa);
				Z = null;
				H();
			})();
		} else {
			H();
		}
	}
	function H() {
		var ag = o.length;
		if (ag > 0) {
			for (var af = 0; af < ag; af++) {
				var Y = o[af].id;
				var ab = o[af].callbackFn;
				var aa = {
					success: false,
					id: Y
				};
				if (M.pv[0] > 0) {
					var ae = c(Y);
					if (ae) {
						if (F(o[af].swfVersion) && !(M.wk && M.wk < 312)) {
							w(Y, true);
							if (ab) {
								aa.success = true;
								aa.ref = z(Y);
								ab(aa);
							}
						} else {
							if (o[af].expressInstall && A()) {
								var ai = {};
								ai.data = o[af].expressInstall;
								ai.width = ae.getAttribute("width") || "0";
								ai.height = ae.getAttribute("height") || "0";
								if (ae.getAttribute("class")) {
									ai.styleclass = ae.getAttribute("class");
								}
								if (ae.getAttribute("align")) {
									ai.align = ae.getAttribute("align");
								}
								var ah = {};
								var X = ae.getElementsByTagName("param");
								var ac = X.length;
								for (var ad = 0; ad < ac; ad++) {
									if (X[ad].getAttribute("name").toLowerCase() !== "movie") {
										ah[X[ad].getAttribute("name")] = X[ad].getAttribute("value");
									}
								}
								P(ai, ah, Y, ab);
							} else {
								p(ae);
								if (ab) {
									ab(aa);
								}
							}
						}
					}
				} else {
					w(Y, true);
					if (ab) {
						var Z = z(Y);
						if (Z && typeof Z.SetVariable !== D) {
							aa.success = true;
							aa.ref = Z;
						}
						ab(aa);
					}
				}
			}
		}
	}
	function z(aa) {
		var X = null;
		var Y = c(aa);
		if (Y && Y.nodeName === "OBJECT") {
			if (typeof Y.SetVariable !== D) {
				X = Y;
			} else {
				var Z = Y.getElementsByTagName(r)[0];
				if (Z) {
					X = Z;
				}
			}
		}
		return X;
	}
	function A() {
		return !a && F("6.0.65") && (M.win || M.mac) && !(M.wk && M.wk < 312);
	}
	function P(aa, ab, X, Z) {
		a = true;
		E = Z || null;
		B = {
			success: false,
			id: X
		};
		var ae = c(X);
		if (ae) {
			if (ae.nodeName === "OBJECT") {
				l = g(ae);
				Q = null;
			} else {
				l = ae;
				Q = X;
			}
			aa.id = R;
			if (typeof aa.width === D || (!/%$/.test(aa.width) && parseInt(aa.width, 10) < 310)) {
				aa.width = "310";
			}
			if (typeof aa.height === D || (!/%$/.test(aa.height) && parseInt(aa.height, 10) < 137)) {
				aa.height = "137";
			}
			j.title = j.title.slice(0, 47) + " - Flash Player Installation";
			var ad = M.ie && M.win ? "ActiveX" : "PlugIn",
				ac = "MMredirectURL=" + O.location.toString().replace(/&/g, "%26") + "&MMplayerType=" + ad + "&MMdoctitle=" + j.title;
			if (typeof ab.flashvars !== D) {
				ab.flashvars += "&" + ac;
			} else {
				ab.flashvars = ac;
			}
			if (M.ie && M.win && ae.readyState !== 4) {
				var Y = C("div");
				X += "SWFObjectNew";
				Y.setAttribute("id", X);
				ae.parentNode.insertBefore(Y, ae);
				ae.style.display = "none"; (function () {
					if (ae.readyState === 4) {
						ae.parentNode.removeChild(ae);
					} else {
						setTimeout(arguments.callee, 10);
					}
				})();
			}
			u(aa, ab, X);
		}
	}
	function p(Y) {
		if (M.ie && M.win && Y.readyState !== 4) {
			var X = C("div");
			Y.parentNode.insertBefore(X, Y);
			X.parentNode.replaceChild(g(Y), X);
			Y.style.display = "none"; (function () {
				if (Y.readyState === 4) {
					Y.parentNode.removeChild(Y);
				} else {
					setTimeout(arguments.callee, 10);
				}
			})();
		} else {
			Y.parentNode.replaceChild(g(Y), Y);
		}
	}
	function g(ab) {
		var aa = C("div");
		if (M.win && M.ie) {
			aa.innerHTML = ab.innerHTML;
		} else {
			var Y = ab.getElementsByTagName(r)[0];
			if (Y) {
				var ad = Y.childNodes;
				if (ad) {
					var X = ad.length;
					for (var Z = 0; Z < X; Z++) {
						if (!(ad[Z].nodeType === 1 && ad[Z].nodeName === "PARAM") && !(ad[Z].nodeType === 8)) {
							aa.appendChild(ad[Z].cloneNode(true));
						}
					}
				}
			}
		}
		return aa;
	}
	function u(ai, ag, Y) {
		var X, aa = c(Y);
		if (M.wk && M.wk < 312) {
			return X;
		}
		if (aa) {
			if (typeof ai.id === D) {
				ai.id = Y;
			}
			if (M.ie && M.win) {
				var ah = "";
				for (var ae in ai) {
					if (ai[ae] !== Object.prototype[ae]) {
						if (ae.toLowerCase() === "data") {
							ag.movie = ai[ae];
						} else {
							if (ae.toLowerCase() === "styleclass") {
								ah += ' class="' + ai[ae] + '"';
							} else {
								if (ae.toLowerCase() !== "classid") {
									ah += " " + ae + '="' + ai[ae] + '"';
								}
							}
						}
					}
				}
				var af = "";
				for (var ad in ag) {
					if (ag[ad] !== Object.prototype[ad]) {
						af += '<param name="' + ad + '" value="' + ag[ad] + '" />';
					}
				}
				aa.outerHTML = '<object classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000"' + ah + ">" + af + "</object>";
				N[N.length] = ai.id;
				X = c(ai.id);
			} else {
				var Z = C(r);
				Z.setAttribute("type", q);
				for (var ac in ai) {
					if (ai[ac] !== Object.prototype[ac]) {
						if (ac.toLowerCase() === "styleclass") {
							Z.setAttribute("class", ai[ac]);
						} else {
							if (ac.toLowerCase() !== "classid") {
								Z.setAttribute(ac, ai[ac]);
							}
						}
					}
				}
				for (var ab in ag) {
					if (ag[ab] !== Object.prototype[ab] && ab.toLowerCase() !== "movie") {
						e(Z, ab, ag[ab]);
					}
				}
				aa.parentNode.replaceChild(Z, aa);
				X = Z;
			}
		}
		return X;
	}
	function e(Z, X, Y) {
		var aa = C("param");
		aa.setAttribute("name", X);
		aa.setAttribute("value", Y);
		Z.appendChild(aa);
	}
	function y(Y) {
		var X = c(Y);
		if (X && X.nodeName === "OBJECT") {
			if (M.ie && M.win) {
				X.style.display = "none"; (function () {
					if (X.readyState === 4) {
						b(Y);
					} else {
						setTimeout(arguments.callee, 10);
					}
				})();
			} else {
				X.parentNode.removeChild(X);
			}
		}
	}
	function b(Z) {
		var Y = c(Z);
		if (Y) {
			for (var X in Y) {
				if (typeof Y[X] === "function") {
					Y[X] = null;
				}
			}
			Y.parentNode.removeChild(Y);
		}
	}
	function c(Z) {
		var X = null;
		try {
			X = j.getElementById(Z);
		} catch (Y) { Console.log(Y); }
		return X;
	}
	function C(X) {
		return j.createElement(X);
	}
	function i(Z, X, Y) {
		Z.attachEvent(X, Y);
		I[I.length] = [Z, X, Y];
	}
	function F(Z) {
		var Y = M.pv,
			X = Z.split(".");
		X[0] = parseInt(X[0], 10);
		X[1] = parseInt(X[1], 10) || 0;
		X[2] = parseInt(X[2], 10) || 0;
		return (Y[0] > X[0] || (Y[0] === X[0] && Y[1] > X[1]) || (Y[0] === X[0] && Y[1] === X[1] && Y[2] >= X[2])) ? true : false;
	}
	function v(ac, Y, ad, ab) {
		if (M.ie && M.mac) {
			return;
		}
		var aa = j.getElementsByTagName("head")[0];
		if (!aa) {
			return;
		}
		var X = (ad && typeof ad === "string") ? ad : "screen";
		if (ab) {
			n = null;
			G = null;
		}
		if (!n || G !== X) {
			var Z = C("style");
			Z.setAttribute("type", "text/css");
			Z.setAttribute("media", X);
			n = aa.appendChild(Z);
			if (M.ie && M.win && typeof j.styleSheets !== D && j.styleSheets.length > 0) {
				n = j.styleSheets[j.styleSheets.length - 1];
			}
			G = X;
		}
		if (M.ie && M.win) {
			if (n && typeof n.addRule === r) {
				n.addRule(ac, Y);
			}
		} else {
			if (n && typeof j.createTextNode !== D) {
				n.appendChild(j.createTextNode(ac + " {" + Y + "}"));
			}
		}
	}
	function w(Z, X) {
		if (!m) {
			return;
		}
		var Y = X ? "visible" : "hidden";
		if (J && c(Z)) {
			c(Z).style.visibility = Y;
		} else {
			v("#" + Z, "visibility:" + Y);
		}
	}
	function L(Y) {
		var Z = /[\\\"<>\.;]/;
		var X = Z.exec(Y) !== null;
		return X && typeof encodeURIComponent !== D ? encodeURIComponent(Y) : Y;
	}
	var d = function () {
		if (M.ie && M.win) {
			window.attachEvent("onunload",
				function () {
					var ac = I.length;
					for (var ab = 0; ab < ac; ab++) {
						I[ab][0].detachEvent(I[ab][1], I[ab][2]);
					}
					var Z = N.length;
					for (var aa = 0; aa < Z; aa++) {
						y(N[aa]);
					}
					for (var Y in M) {
						M[Y] = null;
					}
					M = null;
					for (var X in swfobject) {
						swfobject[X] = null;
					}
					swfobject = null;
				});
		}
	}();
	return {
		registerObject: function (ab, X, aa, Z) {
			if (M.w3 && ab && X) {
				var Y = {};
				Y.id = ab;
				Y.swfVersion = X;
				Y.expressInstall = aa;
				Y.callbackFn = Z;
				o[o.length] = Y;
				w(ab, false);
			} else {
				if (Z) {
					Z({
						success: false,
						id: ab
					});
				}
			}
		},
		getObjectById: function (X) {
			if (M.w3) {
				return z(X);
			}
		},
		embedSWF: function (ab, ah, ae, ag, Y, aa, Z, ad, af, ac) {
			var X = {
				success: false,
				id: ah
			};
			if (M.w3 && !(M.wk && M.wk < 312) && ab && ah && ae && ag && Y) {
				w(ah, false);
				K(function () {
					ae += "";
					ag += "";
					var aj = {};
					if (af && typeof af === r) {
						for (var al in af) {
							aj[al] = af[al];
						}
					}
					aj.data = ab;
					aj.width = ae;
					aj.height = ag;
					var am = {};
					if (ad && typeof ad === r) {
						for (var ak in ad) {
							am[ak] = ad[ak];
						}
					}
					if (Z && typeof Z === r) {
						for (var ai in Z) {
							if (typeof am.flashvars !== D) {
								am.flashvars += "&" + ai + "=" + Z[ai];
							} else {
								am.flashvars = ai + "=" + Z[ai];
							}
						}
					}
					if (F(Y)) {
						var an = u(aj, am, ah);
						if (aj.id === ah) {
							w(ah, true);
						}
						X.success = true;
						X.ref = an;
					} else {
						if (aa && A()) {
							aj.data = aa;
							P(aj, am, ah, ac);
							return;
						} else {
							w(ah, true);
						}
					}
					if (ac) {
						ac(X);
					}
				});
			} else {
				if (ac) {
					ac(X);
				}
			}
		},
		switchOffAutoHideShow: function () {
			m = false;
		},
		ua: M,
		getFlashPlayerVersion: function () {
			return {
				major: M.pv[0],
				minor: M.pv[1],
				release: M.pv[2]
			};
		},
		hasFlashPlayerVersion: F,
		createSWF: function (Z, Y, X) {
			if (M.w3) {
				return u(Z, Y, X);
			} else {
				return undefined;
			}
		},
		showExpressInstall: function (Z, aa, X, Y) {
			if (M.w3 && A()) {
				P(Z, aa, X, Y);
			}
		},
		removeSWF: function (X) {
			if (M.w3) {
				y(X);
			}
		},
		createCSS: function (aa, Z, Y, X) {
			if (M.w3) {
				v(aa, Z, Y, X);
			}
		},
		addDomLoadEvent: K,
		addLoadEvent: s,
		getQueryParamValue: function (aa) {
			var Z = j.location.search || j.location.hash;
			if (Z) {
				if (/\?/.test(Z)) {
					Z = Z.split("?")[1];
				}
				if (aa === null) {
					return L(Z);
				}
				var Y = Z.split("&");
				for (var X = 0; X < Y.length; X++) {
					if (Y[X].substring(0, Y[X].indexOf("=")) === aa) {
						return L(Y[X].substring((Y[X].indexOf("=") + 1)));
					}
				}
			}
			return "";
		},
		expressInstallCallback: function () {
			if (a) {
				var X = c(R);
				if (X && l) {
					X.parentNode.replaceChild(l, X);
					if (Q) {
						w(Q, true);
						if (M.ie && M.win) {
							l.style.display = "block";
						}
					}
					if (E) {
						E(B);
					}
				}
				a = false;
			}
		}
	};
}();

/**
 * 分页插件 jquery_pagination 修改版
 * http://hooray.github.com/jquery.pagination/
 */
(function ($) {
	$.PaginationCalculator = function (maxentries, opts) {
		this.maxentries = maxentries;
		this.opts = opts;
	};
	$.extend($.PaginationCalculator.prototype, {
		numPages: function () {
			return Math.ceil(this.maxentries / this.opts.items_per_page);
		},
		getInterval: function (current_page) {
			var ne_half = Math.floor(this.opts.num_display_entries / 2);
			var np = this.numPages();
			var upper_limit = np - this.opts.num_display_entries;
			var start = current_page > ne_half ? Math.max(Math.min(current_page - ne_half, upper_limit), 0) : 0;
			var end = current_page > ne_half ? Math.min(current_page + ne_half + (this.opts.num_display_entries % 2), np) : Math.min(this.opts.num_display_entries, np);
			return {
				start: start,
				end: end
			};
		}
	});
	$.PaginationRenderers = {};
	$.PaginationRenderers.defaultRenderer = function (maxentries, opts) {
		this.maxentries = maxentries;
		this.opts = opts;
		this.pc = new $.PaginationCalculator(maxentries, opts);
	};
	$.extend($.PaginationRenderers.defaultRenderer.prototype, {
		createLink: function (page_id, current_page, appendopts) {
			var lnk, np = this.pc.numPages();
			page_id = page_id < 0 ? 0 : (page_id < np ? page_id : np - 1);
			appendopts = $.extend({
				text: page_id + 1,
				classes: ""
			},
				appendopts || {});
			if (page_id === current_page) {
				if (isNaN(appendopts.text)) {
					lnk = $("<li class='disabled page-item'><a class='page-link'>" + appendopts.text + "</a></li>");
				} else {
					lnk = $("<li class='active page-item'><a class='page-link'>" + appendopts.text + "</a></li>");
				}
			} else {
				lnk = $("<li><a class='page-link' href='" + this.opts.link_to.replace(/__id__/, page_id) + "'>" + appendopts.text + "</a></li>");
			}
			if (appendopts.classes) {
				lnk.addClass(appendopts.classes);
			}
			lnk.data("page_id", page_id);
			return lnk;
		},
		appendRange: function (container, current_page, start, end, opts) {
			var i;
			for (i = start; i < end; i++) {
				this.createLink(i, current_page, opts).appendTo(container);
			}
		},
		getLinks: function (current_page, eventHandler) {
			current_page = parseInt(current_page);
			var begin, end, interval = this.pc.getInterval(current_page),
				np = this.pc.numPages(),
				fragment = $("<ul class='pagination justify-content-center'></ul>");
			if (this.opts.prev_text && (current_page > 0 || this.opts.prev_show_always)) {
				fragment.append(this.createLink(current_page - 1, current_page, {
					text: this.opts.prev_text,
					classes: "prev page-item"
				}));
			}
			if (interval.start > 0 && this.opts.num_edge_entries > 0) {
				end = Math.min(this.opts.num_edge_entries, interval.start);
				this.appendRange(fragment, current_page, 0, end, {
					classes: "sp"
				});
				if (this.opts.num_edge_entries < interval.start && this.opts.ellipse_text) {
					$("<li class='disabled page-item'><a class='page-link'>" + this.opts.ellipse_text + "</a></li>").appendTo(fragment);
				}
			}
			this.appendRange(fragment, current_page, interval.start, interval.end);
			if (interval.end < np && this.opts.num_edge_entries > 0) {
				if (np - this.opts.num_edge_entries > interval.end && this.opts.ellipse_text) {
					$("<li class='disabled page-item'><a class='page-link'>" + this.opts.ellipse_text + "</a></li>").appendTo(fragment);
				}
				begin = Math.max(np - this.opts.num_edge_entries, interval.end);
				this.appendRange(fragment, current_page, begin, np, {
					classes: "ep"
				});
			}
			if (this.opts.next_text && (current_page < np - 1 || this.opts.next_show_always)) {
				fragment.append(this.createLink(current_page + 1, current_page, {
					text: this.opts.next_text,
					classes: "next page-item"
				}));
			}
			$("li:not(.disabled, .active) a", fragment).click(eventHandler);
			return fragment;
		}
	});
	$.fn.pagination = function (maxentries, opts) {

		opts = $.extend({
			items_per_page: 10,
			num_display_entries: 11,
			current_page: 0,
			num_edge_entries: 0,
			link_to: "###",
			prev_text: "Prev",
			next_text: "Next",
			ellipse_text: "...",
			prev_show_always: true,
			next_show_always: true,
			renderer: "defaultRenderer",
			load_first_page: false,
			callback: function () {
				return false;
			}
		},
			opts || {});
		var containers = this,
			renderer, links, current_page;
		function paginationClickHandler(evt) {
			var links, new_current_page = $(evt.target).parent().data("page_id"),
				continuePropagation = selectPage(new_current_page);
			if (!continuePropagation) {
				evt.stopPropagation();
			}
			return continuePropagation;
		}
		function selectPage(new_current_page) {

			containers.data("current_page", new_current_page);
			links = renderer.getLinks(new_current_page, paginationClickHandler);
			containers.empty();
			links.appendTo(containers);
			var continuePropagation = opts.callback(new_current_page, containers);
			return continuePropagation;
		}
		current_page = opts.current_page;
		containers.data("current_page", current_page);
		maxentries = (!maxentries || maxentries < 0) ? 1 : maxentries;
		opts.items_per_page = (!opts.items_per_page || opts.items_per_page < 0) ? 1 : opts.items_per_page;
		if (!$.PaginationRenderers[opts.renderer]) {
			throw new ReferenceError("Pagination renderer '" + opts.renderer + "' was not found in jQuery.PaginationRenderers object.");
		}
		renderer = new $.PaginationRenderers[opts.renderer](maxentries, opts);
		var pc = new $.PaginationCalculator(maxentries, opts);
		var np = pc.numPages();
		containers.off("setPage").on("setPage", {
			numPages: np
		},
			function (evt, page_id) {
				if (page_id >= 0 && page_id < evt.data.numPages) {
					selectPage(page_id);
					return false;
				}
			});
		containers.off("prevPage").on("prevPage",
			function (evt) {
				var current_page = $(this).data("current_page");
				if (current_page > 0) {
					selectPage(current_page - 1);
				}
				return false;
			});
		containers.off("nextPage").on("nextPage", {
			numPages: np
		},
			function (evt) {
				var current_page = $(this).data("current_page");
				if (current_page < evt.data.numPages - 1) {
					selectPage(current_page + 1);
				}
				return false;
			});
		containers.off("currentPage").on("currentPage",
			function () {
				var current_page = $(this).data("current_page");
				selectPage(current_page);
				return false;
			});
		links = renderer.getLinks(current_page, paginationClickHandler);
		containers.empty();
		links.appendTo(containers);
		if (opts.load_first_page) {
			opts.callback(current_page, containers);
		}
	};
})(jQuery);

NewCrm = {};
NewCrm.info = function (msg) {
	if (!$('.lobibox-info').length) {
		top.Lobibox.alert('info', {
			msg: msg,
			width: 300
		})
	}
};
NewCrm.success = function (msg) {
	if (!$('.lobibox-success').length) {
		top.Lobibox.alert('success', {
			msg: msg,
			width: 300
		})
	}
};
NewCrm.fail = function (msg) {
	if (!$('.lobibox-error').length) {
		top.Lobibox.alert('error', {
			msg: msg || '出现未知错误，请重试',
			width: 300
		})
	}
};
NewCrm.warning = function (msg) {
	if (!$('.lobibox-warning').length) {
		top.Lobibox.alert('warning', {
			msg: msg,
			width: 300
		})
	}
}
let loading = [];
NewCrm.loading = function (msg) {
	// if (!$('.lobibox-loading').length) {
	// 	loading.push(parent.Lobibox.alert('loading', {
	// 		msg: msg,
	// 		width: 300
	// 	}))
	// } 
};
NewCrm.close = function () {
	// for (let index = 0; index < loading.length; index++) {
	// 	let element = loading[index];
	// 	element.hide()
	// }
};

/**
 * 全屏插件
 * http://johndyer.name/native-fullscreen-javascript-api-plus-jquery-plugin/
 */
(function () {
	var d = {
		supportsFullScreen: false,
		isFullScreen: function () {
			return false;
		},
		requestFullScreen: function () { },
		cancelFullScreen: function () { },
		fullScreenEventName: "",
		prefix: ""
	},
		c = "webkit moz o ms khtml".split(" ");
	if (typeof document.cancelFullScreen !== "undefined") {
		d.supportsFullScreen = true;
	} else {
		for (var b = 0,
			a = c.length; b < a; b++) {
			d.prefix = c[b];
			if (typeof document[d.prefix + "CancelFullScreen"] !== "undefined") {
				d.supportsFullScreen = true;
				break;
			}
		}
	}
	if (d.supportsFullScreen) {
		d.fullScreenEventName = d.prefix + "fullscreenchange";
		d.isFullScreen = function () {
			switch (this.prefix) {
				case "":
					return document.fullScreen;
				case "webkit":
					return document.webkitIsFullScreen;
				default:
					return document[this.prefix + "FullScreen"];
			}
		};
		d.requestFullScreen = function (e) {
			return (this.prefix === "") ? e.requestFullScreen() : e[this.prefix + "RequestFullScreen"]();
		};
		d.cancelFullScreen = function (e) {
			return (this.prefix === "") ? document.cancelFullScreen() : document[this.prefix + "CancelFullScreen"]();
		};
	}
	if (typeof jQuery !== "undefined") {
		jQuery.fn.requestFullScreen = function () {
			return this.each(function () {
				if (d.supportsFullScreen) {
					d.requestFullScreen(this);
				}
			});
		};
	}
	window.fullScreenApi = d;
})();

/**
 * artTemplate 3.0 - Template Engine
 * https://github.com/aui/artTemplate
 */
!
	function () {
		function a(a) {
			return a.replace(t, "").replace(u, ",").replace(v, "").replace(w, "").replace(x, "").split(y);
		}
		function b(a) {
			return "'" + a.replace(/('|\\)/g, "\\$1").replace(/\r/g, "\\r").replace(/\n/g, "\\n") + "'";
		}
		function c(c, d) {
			function e(a) {
				return m += a.split(/\n/).length - 1,
					k && (a = a.replace(/\s+/g, " ").replace(/<!--[\w\W]*?-->/g, "")),
					a && (a = s[1] + b(a) + s[2] + "\n"),
					a;
			}
			function f(b) {
				var c = m;
				if (j ? b = j(b, d) : g && (b = b.replace(/\n/g,
					function () {
						return m++ ,
							"$line=" + m + ";";
					})), 0 === b.indexOf("=")) {
					var e = l && !/^=[=#]/.test(b);
					if (b = b.replace(/^=[=#]?|[\s;]*$/g, ""), e) {
						var f = b.replace(/\s*\([^\)]+\)/, "");
						n[f] || /^(include|print)$/.test(f) || (b = "$escape(" + b + ")");
					} else b = "$string(" + b + ")";
					b = s[1] + b + s[2];
				}
				return g && (b = "$line=" + c + ";" + b),
					r(a(b),
						function (a) {
							if (a && !p[a]) {
								var b;
								b = "print" === a ? u : "include" === a ? v : n[a] ? "$utils." + a : o[a] ? "$helpers." + a : "$data." + a,
									w += a + "=" + b + ",",
									p[a] = !0;
							}
						}),
					b + "\n";
			}
			var g = d.debug,
				h = d.openTag,
				i = d.closeTag,
				j = d.parser,
				k = d.compress,
				l = d.escape,
				m = 1,
				p = {
					$data: 1,
					$filename: 1,
					$utils: 1,
					$helpers: 1,
					$out: 1,
					$line: 1
				},
				q = "".trim,
				s = q ? ["$out='';", "$out+=", ";", "$out"] : ["$out=[];", "$out.push(", ");", "$out.join('')"],
				t = q ? "$out+=text;return $out;" : "$out.push(text);",
				u = "function(){var text=''.concat.apply('',arguments);" + t + "}",
				v = "function(filename,data){data=data||$data;var text=$utils.$include(filename,data,$filename);" + t + "}",
				w = "'use strict';var $utils=this,$helpers=$utils.$helpers," + (g ? "$line=0," : ""),
				x = s[0],
				y = "return new String(" + s[3] + ");";
			r(c.split(h),
				function (a) {
					a = a.split(i);
					var b = a[0],
						c = a[1];
					1 === a.length ? x += e(b) : (x += f(b), c && (x += e(c)));
				});
			var z = w + x + y;
			g && (z = "try{" + z + "}catch(e){throw {filename:$filename,name:'Render Error',message:e.message,line:$line,source:" + b(c) + ".split(/\\n/)[$line-1].replace(/^\\s+/,'')};}");
			try {
				var A = new Function("$data", "$filename", z);
				return A.prototype = n,
					A;
			} catch (B) {
				throw B.temp = "function anonymous($data,$filename) {" + z + "}",
				B;
			}
		}
		var d = function (a, b) {
			return "string" === typeof b ? q(b, {
				filename: a
			}) : g(a, b);
		};
		d.version = "3.0.0",
			d.config = function (a, b) {
				e[a] = b;
			};
		var e = d.defaults = {
			openTag: "<%",
			closeTag: "%>",
			escape: !0,
			cache: !0,
			compress: !1,
			parser: null
		},
			f = d.cache = {};
		d.render = function (a, b) {
			return q(a, b);
		};
		var g = d.renderFile = function (a, b) {
			var c = d.get(a) || p({
				filename: a,
				name: "Render Error",
				message: "Template not found"
			});
			return b ? c(b) : c;
		};
		d.get = function (a) {
			var b;
			if (f[a]) b = f[a];
			else if ("object" === typeof document) {
				var c = document.getElementById(a);
				if (c) {
					var d = (c.value || c.innerHTML).replace(/^\s*|\s*$/g, "");
					b = q(d, {
						filename: a
					});
				}
			}
			return b;
		};
		var h = function (a, b) {
			return "string" !== typeof a && (b = typeof a, "number" === b ? a += "" : a = "function" === b ? h(a.call(a)) : ""),
				a;
		},
			i = {
				"<": "&#60;",
				">": "&#62;",
				'"': "&#34;",
				"'": "&#39;",
				"&": "&#38;"
			},
			j = function (a) {
				return i[a];
			},
			k = function (a) {
				return h(a).replace(/&(?![\w#]+;)|[<>"']/g, j);
			},
			l = Array.isArray ||
				function (a) {
					return "[object Array]" === {}.toString.call(a);
				},
			m = function (a, b) {
				var c, d;
				if (l(a)) for (c = 0, d = a.length; d > c; c++) b.call(a, a[c], c, a);
				else for (c in a) b.call(a, a[c], c);
			},
			n = d.utils = {
				$helpers: {},
				$include: g,
				$string: h,
				$escape: k,
				$each: m
			};
		d.helper = function (a, b) {
			o[a] = b;
		};
		var o = d.helpers = n.$helpers;
		d.onerror = function (a) {
			var b = "Template Error\n\n";
			for (var c in a) b += "<" + c + ">\n" + a[c] + "\n\n";
			"object" === typeof console && console.error(b);
		};
		var p = function (a) {
			return d.onerror(a),
				function () {
					return "{Template Error}";
				};
		},
			q = d.compile = function (a, b) {
				function d(c) {
					try {
						return new i(c, h) + "";
					} catch (d) {
						return b.debug ? p(d)() : (b.debug = !0, q(a, b)(c));
					}
				}
				b = b || {};
				for (var g in e) void 0 === b[g] && (b[g] = e[g]);
				var h = b.filename;
				try {
					var i = c(a, b);
				} catch (j) {
					return j.filename = h || "anonymous",
						j.name = "Syntax Error",
						p(j);
				}
				return d.prototype = i.prototype,
					d.toString = function () {
						return i.toString();
					},
					h && b.cache && (f[h] = d),
					d;
			},
			r = n.$each,
			s = "break,case,catch,continue,debugger,default,delete,do,else,false,finally,for,function,if,in,instanceof,new,null,return,switch,this,throw,true,try,typeof,var,void,while,with,abstract,boolean,byte,char,class,const,double,enum,export,extends,final,float,goto,implements,import,int,interface,long,native,package,private,protected,public,short,static,super,synchronized,throws,transient,volatile,arguments,let,yield,undefined",
			t = /\/\*[\w\W]*?\*\/|\/\/[^\n]*\n|\/\/[^\n]*$|"(?:[^"\\]|\\[\w\W])*"|'(?:[^'\\]|\\[\w\W])*'|\s*\.\s*[$\w\.]+/g,
			u = /[^\w$]+/g,
			v = new RegExp(["\\b" + s.replace(/,/g, "\\b|\\b") + "\\b"].join("|"), "g"),
			w = /^\d[^,]*|,\d[^,]*/g,
			x = /^,+|,+$/g,
			y = /^$|,+/;
		"function" === typeof define ? define(function () {
			return d;
		}) : "undefined" !== typeof exports ? module.exports = d : this.template = d;
	}();