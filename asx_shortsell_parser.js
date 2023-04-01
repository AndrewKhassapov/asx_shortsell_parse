//Parses the ASX short-sell list into companies, tickers and percentage of value shorted.

fetch('https://www.asx.com.au/data/shortsell.txt').then(function (response) {

    return response.text(); //Fetch call succesful.

}).then(function (html) {

    //let parser = new DOMParser();
    //let htmlBody =  parser.parseFromString(html, 'text/html');

    //let txtElement = document.getElementsByTagName('pre');
    //let txt = txtElement[0].textContent;
    let txtArray = html.split('\n'); //txt.split('\n');
    let regexMatch = /([\.\,\:a-zA-Z0-9_])+/gi;

    class Company {
        constructor(line) {
            this.ticker = this.readLineIndex(line, 0);
            this.soldPercent = Number(this.readLineIndex(line, -1));
            this.volume = this.readLineIndex(line, -3);
            this.capital = this.readLineIndex(line, -2);
        };

        readLineIndex(line, index) {
            let lineMatch = line.match(regexMatch);
            if (lineMatch == null) { return ''; };

            index = (index >= 0) ? index : lineMatch.length + index;

            if (index < lineMatch.length) {
                return lineMatch[index];
            }
            return '';
        }
    }

    function compareByPercent(companyA, companyB) {
        if (companyA.soldPercent < companyB.soldPercent) {
            return -1;
        }
        if (companyA.soldPercent > companyB.soldPercent) {
            return 1;
        }
        return 0;
    }

    function parseLines() {
        let parsedLines = [];

        for (let i = 0; i < txtArray.length; i++) {
            parsedLines[i] = new Company(txtArray[i]);
        }

        return parsedLines;
    }

    function printLine(element) {
        if ((typeof Number(element.soldPercent) === "number")) {
            console.log(element);
        }
    }


    let parsedLines = parseLines();
    parsedLines.sort(compareByPercent);
    //parsedLines.forEach(e => console.log(e));
    parsedLines.forEach(printLine);

}).catch(function (err) {
    console.warn('Warning: not able to fetch URL due to ' + err); //Error report.
});