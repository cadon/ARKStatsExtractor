// You can use it in your browser to get kibbles.json from http://ark.gamepedia.com/Kibble

var table = $('.wikitable')[1];
var len = table.rows.length;
var kibbles = {};
for(var i = 1; i < len; i++){
	var row = table.rows[i];

	var egg = row.cells[0].innerText.replace(/^\s*/,"").replace(/\s*\(.*/, "").replace(/\d*px\s*/,"");
	var name = egg.replace(/\s*Egg.*/, "");

	var food1 = row.cells[1].innerText.replace(/^\s*/,"");
	var food1_amount = 1;
	var match = food1.match(/(\d*)\s*[x×]/);
	if(match != null){
		food1_amount = match[1];
		food1 = food1.replace(/(\d*)\s*[x×]\s*/, "");
	}

	var food2 = row.cells[2].innerText.replace(/^\s*/,"");
	var food2_amount = 1;
	match = food2.match(/(\d*)\s*[x×]/);
	if(match != null){
		food2_amount = match[1];
		food2 = food2.replace(/(\d*)\s*[x×]\s*/, "");
	}
	
	var kibble = {};
	kibble[egg] = 1;
	kibble[food1] = food1_amount;
	kibble[food2] = food2_amount;
	if(name == "Quetzal"){
		kibble["Mejoberries"] = 100;
		kibble["Fiber"] = 120;
	} else {
		kibble["Mejoberries"] = 2;
		kibble["Fiber"] = 3;
	}
	kibbles[name] = kibble;
}
JSON.stringify({
  ver: "259.0",
  "kibbles": kibbles
});