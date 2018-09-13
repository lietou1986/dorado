/**
 * EasyUI for jQuery 1.5.5.6
 * 
 * Copyright (c) 2009-2018 www.jeasyui.com. All rights reserved.
 *
 * Licensed under the freeware license: http://www.jeasyui.com/license_freeware.php
 * To use it on other terms please contact us: info@jeasyui.com
 *
 */
(function($){
var _1=1;
function _2(_3){
$(_3).addClass("sidemenu");
};
function _4(_5,_6){
var _7=$(_5).sidemenu("options");
if(_6){
$.extend(_7,{width:_6.width,height:_6.height});
}
$(_5)._size(_7);
$(_5).find(".accordion").accordion("resize");
};
function _8(_9,_a,_b){
var _c=$(_9).sidemenu("options");
var tt=$("<ul class=\"sidemenu-tree\"></ul>").appendTo(_a);
tt.tree({data:_b,animate:_c.animate,onBeforeSelect:function(_d){
if(_d.children){
return false;
}
},onSelect:function(_e){
_12(_9,_e.id);
},onExpand:function(_f){
_22(_9,_f);
},onCollapse:function(_10){
_22(_9,_10);
},onClick:function(_11){
if(_11.children){
if(_11.state=="open"){
$(_11.target).addClass("tree-node-nonleaf-collapsed");
}else{
$(_11.target).removeClass("tree-node-nonleaf-collapsed");
}
$(this).tree("toggle",_11.target);
}
}});
tt.unbind(".sidemenu").bind("mouseleave.sidemenu",function(){
$(_a).trigger("mouseleave");
});
_12(_9,_c.selectedItemId);
};
function _13(_14,_15,_16){
var _17=$(_14).sidemenu("options");
$(_15).tooltip({content:$("<div></div>"),position:_17.floatMenuPosition,valign:"top",data:_16,onUpdate:function(_18){
var _19=$(this).tooltip("options");
var _1a=_19.data;
_18.accordion({width:_17.floatMenuWidth,multiple:false}).accordion("add",{title:_1a.text,collapsed:false,collapsible:false});
_8(_14,_18.accordion("panels")[0],_1a.children);
},onShow:function(){
var t=$(this);
var tip=t.tooltip("tip").addClass("sidemenu-tooltip");
tip.children(".tooltip-content").addClass("sidemenu");
tip.find(".accordion").accordion("resize");
tip.unbind().bind("mouseenter",function(){
t.tooltip("show");
}).bind("mouseleave",function(){
t.tooltip("hide");
});
},onPosition:function(){
if(!_17.collapsed){
$(this).tooltip("tip").css({left:-999999});
}
}});
};
function _1b(_1c,_1d){
$(_1c).find(".sidemenu-tree").each(function(){
_1d($(this));
});
$(_1c).find(".tooltip-f").each(function(){
var tip=$(this).tooltip("tip");
if(tip){
tip.find(".sidemenu-tree").each(function(){
_1d($(this));
});
}
});
};
function _12(_1e,_1f){
var _20=$(_1e).sidemenu("options");
_1b(_1e,function(t){
t.find("div.tree-node-selected").removeClass("tree-node-selected");
var _21=t.tree("find",_1f);
if(_21){
$(_21.target).addClass("tree-node-selected");
_20.selectedItemId=_21.id;
t.trigger("mouseleave");
_20.onSelect.call(_1e,_21);
}
});
};
function _22(_23,_24){
_1b(_23,function(t){
var _25=t.tree("find",_24.id);
if(_25){
t.tree(_24.state=="open"?"expand":"collapse",_25.target);
}
});
};
function _26(_27){
var _28=$(_27).sidemenu("options");
$(_27).empty();
if(_28.data){
$.easyui.forEach(_28.data,true,function(_29){
if(!_29.id){
_29.id="_easyui_sidemenu_"+(_1++);
}
if(!_29.iconCls){
_29.iconCls="sidemenu-default-icon";
}
if(_29.children){
_29.nodeCls="tree-node-nonleaf";
if(!_29.state){
_29.state="closed";
}
if(_29.state=="open"){
_29.nodeCls="tree-node-nonleaf";
}else{
_29.nodeCls="tree-node-nonleaf tree-node-nonleaf-collapsed";
}
}
});
var acc=$("<div></div>").appendTo(_27);
acc.accordion({fit:_28.height=="auto"?false:true,border:_28.border,multiple:_28.multiple});
var _2a=_28.data;
for(var i=0;i<_2a.length;i++){
acc.accordion("add",{title:_2a[i].text,selected:_2a[i].state=="open",iconCls:_2a[i].iconCls});
var ap=acc.accordion("panels")[i];
_8(_27,ap,_2a[i].children);
_13(_27,ap.panel("header"),_2a[i]);
}
}
};
function _2b(_2c,_2d){
var _2e=$(_2c).sidemenu("options");
_2e.collapsed=_2d;
var acc=$(_2c).find(".accordion");
var _2f=acc.accordion("panels");
acc.accordion("options").animate=false;
if(_2e.collapsed){
$(_2c).addClass("sidemenu-collapsed");
for(var i=0;i<_2f.length;i++){
var _30=_2f[i];
if(_30.panel("options").collapsed){
_2e.data[i].state="closed";
}else{
_2e.data[i].state="open";
acc.accordion("unselect",i);
}
var _31=_30.panel("header");
_31.find(".panel-title").html("");
_31.find(".panel-tool").hide();
}
}else{
$(_2c).removeClass("sidemenu-collapsed");
for(var i=0;i<_2f.length;i++){
var _30=_2f[i];
if(_2e.data[i].state=="open"){
acc.accordion("select",i);
}
var _31=_30.panel("header");
_31.find(".panel-title").html(_30.panel("options").title);
_31.find(".panel-tool").show();
}
}
acc.accordion("options").animate=_2e.animate;
};
function _32(_33){
$(_33).find(".tooltip-f").each(function(){
$(this).tooltip("destroy");
});
$(_33).remove();
};
$.fn.sidemenu=function(_34,_35){
if(typeof _34=="string"){
var _36=$.fn.sidemenu.methods[_34];
return _36(this,_35);
}
_34=_34||{};
return this.each(function(){
var _37=$.data(this,"sidemenu");
if(_37){
$.extend(_37.options,_34);
}else{
_37=$.data(this,"sidemenu",{options:$.extend({},$.fn.sidemenu.defaults,$.fn.sidemenu.parseOptions(this),_34)});
_2(this);
}
_4(this);
_26(this);
_2b(this,_37.options.collapsed);
});
};
$.fn.sidemenu.methods={options:function(jq){
return jq.data("sidemenu").options;
},resize:function(jq,_38){
return jq.each(function(){
_4(this,_38);
});
},collapse:function(jq){
return jq.each(function(){
_2b(this,true);
});
},expand:function(jq){
return jq.each(function(){
_2b(this,false);
});
},destroy:function(jq){
return jq.each(function(){
_32(this);
});
}};
$.fn.sidemenu.parseOptions=function(_39){
var t=$(_39);
return $.extend({},$.parser.parseOptions(_39,["width","height"]));
};
$.fn.sidemenu.defaults={width:200,height:"auto",border:true,animate:true,multiple:true,collapsed:false,data:null,floatMenuWidth:200,floatMenuPosition:"right",onSelect:function(_3a){
}};
})(jQuery);

