package main

import (
	"encoding/csv"
	"errors"
	"fmt"
	"io"
	"log"
	"math/rand"
	"os"
	"path"
	"strconv"
	"strings"
	"time"
	"unicode/utf8"
)

func fldByName(fldName string, hdr []string, rec []string) (string, error) {
	for idx := 0; idx < len(hdr); idx++ {
		if strings.Compare(fldName, hdr[idx]) == 0 {
			return rec[idx], nil
		}
	}
	return "", errors.New("missing field")
}

func fldIdxByName(fldName string, hdr []string) (int, error) {
	for idx := 0; idx < len(hdr); idx++ {
		//fmt.Printf("comparing %s == %s\n", fldName, hdr[idx])
		if strings.Compare(strings.ToUpper(fldName), strings.ToUpper(hdr[idx])) == 0 {
			//fmt.Printf("%s has index %d\n", fldName, idx)
			return idx, nil
		}
	}
	return -1, errors.New("missing field")
}

const (
	Mandatory    = iota
	NonMandatory = iota
	MandatoryIf  = iota
)

type Field struct {
	Model         string
	FileName      string
	FieldName     string
	DefinedOnLine int
	DomainType    string
	MinLen        int
	MaxLen        int
	Mandatory     int
	Context       string
	RealMandatory int
}

func parseMandatory(val string) int {
	if strings.Compare(strings.ToLower(val), "mandatory") == 0 {
		return Mandatory
	}
	if strings.Compare(strings.ToLower(val), "mandatoryif") == 0 {
		return MandatoryIf
	}
	return NonMandatory
}

func fileNotPresent(files []string, file string) bool {
	for _, f := range files {
		if strings.Compare(strings.ToLower(f), strings.ToLower(file)) == 0 {
			return false
		}
	}
	return true
}

var codesByField map[string][]string

func main() {

	rand.Seed(time.Now().UnixNano())

	if len(os.Args) < 4 {
		log.Fatal("Usage: package-generator fields_structure.csv codebook.csv model_game_file.csv fileLineCount")
	}

	codesByField = make(map[string][]string)

	csvSchema := os.Args[1]
	codebooks := os.Args[2]
	modelGameFile := os.Args[3]
	fileLineCount, _ := strconv.Atoi(os.Args[4])

	filesByModel, fieldsByFileModel, _ := loadFields(csvSchema)
	filesByModelGame, _ := loadModelGameFile(modelGameFile)
	codesByField, _ = loadCodebooks(codebooks)

	// fmt.Printf("%v\n", filesByModel)
	//fmt.Printf("%v\n", fieldsByFileModel)
	// fmt.Printf("%v\n", codesByField)
	var err error
	err = generatePackage(".", "28934929", "V", "L", "", "2019", "06", "12", "08", "01", fileLineCount, filesByModel, fieldsByFileModel, codesByField, filesByModelGame)
	if err != nil {
		log.Fatalf("error creating package %s", err.Error())
	}

	err = generatePackage(".", "28934929", "V", "K", "", "2019", "06", "12", "08", "01", fileLineCount, filesByModel, fieldsByFileModel, codesByField, filesByModelGame)
	if err != nil {
		log.Fatalf("error creating package %s", err.Error())
	}

	err = generatePackage(".", "28934929", "V", "R", "", "2019", "06", "12", "08", "01", fileLineCount, filesByModel, fieldsByFileModel, codesByField, filesByModelGame)
	if err != nil {
		log.Fatalf("error creating package %s", err.Error())
	}

	err = generatePackage(".", "28934929", "V", "B", "", "2019", "06", "12", "08", "01", fileLineCount, filesByModel, fieldsByFileModel, codesByField, filesByModelGame)
	if err != nil {
		log.Fatalf("error creating package %s", err.Error())
	}

	err = generatePackage(".", "28934929", "V", "T", "", "2019", "06", "12", "08", "01", fileLineCount, filesByModel, fieldsByFileModel, codesByField, filesByModelGame)
	if err != nil {
		log.Fatalf("error creating package %s", err.Error())
	}

	err = generatePackage(".", "28934929", "V", "Z", "", "2019", "06", "12", "08", "01", fileLineCount, filesByModel, fieldsByFileModel, codesByField, filesByModelGame)
	if err != nil {
		log.Fatalf("error creating package %s", err.Error())
	}

	//=============M
	err = generatePackage(".", "28934929", "M", "B", "cas01", "2019", "06", "12", "08", "01", fileLineCount, filesByModel, fieldsByFileModel, codesByField, filesByModelGame)
	if err != nil {
		log.Fatalf("error creating package %s", err.Error())
	}

	err = generatePackage(".", "28934929", "M", "Z", "cas01", "2019", "06", "12", "08", "01", fileLineCount, filesByModel, fieldsByFileModel, codesByField, filesByModelGame)
	if err != nil {
		log.Fatalf("error creating package %s", err.Error())
	}

}

func generatePackage(baseDir string, idProvozovatele string, model string, druhHry string,
	kasino string, rok string, mesic string, den string, hodina string, verze string, dataLineCount int,
	filesByModel map[string][]string, fieldsByFileModel map[string][]Field,
	codesByField map[string][]string, filesByModelGame map[string][]string) error {
	var packageName string
	if kasino != "" {
		packageName = fmt.Sprintf("%s-%s-%s%s-%s-%s-%s", idProvozovatele, model, rok, mesic, druhHry, kasino, verze)
	} else {
		packageName = fmt.Sprintf("%s-%s-%s%s%s%s-%s-%s", idProvozovatele, model, rok, mesic, den, hodina, druhHry, verze)
	}
	//fmt.Printf("package name: %s\n", packageName)
	targetDir := path.Join(baseDir, packageName)
	os.MkdirAll(targetDir, 0777)
	fileList := filesByModelGame[fmt.Sprintf("%s-%s", strings.ToLower(model), strings.ToLower(druhHry))]
	for _, file := range fileList {
		filePath := path.Join(targetDir, file)
		//fmt.Printf("creating file %s\n", filePath)
		fields := fieldsByFileModel[strings.ToLower(fmt.Sprintf("%s-%s", model, file))]
		out, err := os.Create(filePath)
		if err != nil {
			return err
		}
		defer func() { out.Close() }()

		//write metaline
		_, err = fmt.Fprintf(out, "#%s;%s;%s;%s\r\n", packageName, file, time.Now().Format(time.RFC3339), "1.0")
		if err != nil {
			return err
		}

		//write header
		for fldIdx, fld := range fields {

			if fldIdx > 0 {
				_, err = fmt.Fprint(out, ";")
			}
			if err != nil {
				return err
			}

			_, err = fmt.Fprint(out, fld.FieldName)
			if err != nil {
				return err
			}
		}
		_, err = fmt.Fprintf(out, "\r\n")
		if err != nil {
			return err
		}

		fieldValues := make([]string, len(fields), len(fields))
		//generate data
		for lineNo := 0; lineNo < dataLineCount; lineNo++ {

			// conditionals
			/*
				CageTableTournamentBingo.NotIn('P')
				CageTableTournamentBingo.NotIn('P', 'T', 'B')
				CashOut.Equals(1)
				Field.IsEmpty(IDMainEvent)
				SidloKodRuian.IsEmpty()
				TransactionType.NotIn(‘2’, ‘3’, ‘4’, ‘6’, ‘7’, ‘L1’, ‘8’, ‘9’, ‘10’, ‘11’, ‘12’, ‘13’, ‘15’, ‘16’ or ‘17’)
				TypeSM.(‘f’, 'g')
				TypeSM.In(‘a’, ‘b’, ‘c’ or ‘d’)
				TypeSM.In(‘e’)

			*/

			// gen values
			for fldIdx, fld := range fields {
				fieldValues[fldIdx] = generateFieldValue(fld)
			}

			// modify according to context
			for fldIdx, fld := range fields {
				if strings.Compare(fld.Context, "") != 0 {
					switch strings.ToUpper(fld.Context) {
					case "EQUALSGAMETYPE":
						fieldValues[fldIdx] = druhHry
					case "EQUALSOPERATORID":
						fieldValues[fldIdx] = idProvozovatele
					case "STARTSWITHOPERATORID":
						fieldValues[fldIdx] = fmt.Sprintf("%s-%s", idProvozovatele, fieldValues[fldIdx])
						//case "WITHINPACKAGETIMESPAN":
					}
				}
			}

			for fldIdx := range fields {

				if fldIdx > 0 {
					_, err = fmt.Fprint(out, ";")
				}
				if err != nil {
					return err
				}

				_, err = fmt.Fprint(out, addQuotes(fieldValues[fldIdx]))
				if err != nil {
					return err
				}
			}
			_, err = fmt.Fprintf(out, "\r\n")
			if err != nil {
				return err
			}
		}

		out.Close()
	}
	return nil
}

func escapeQuotes(inp string) string {
	if strings.Contains(inp, "\"") {
		return strings.Replace(inp, "\"", "\"\"", 0)
	}
	return inp
}

func addQuotes(inp string) string {
	if strings.Contains(inp, ";") ||
		strings.Contains(inp, "\r") ||
		strings.Contains(inp, "\n") {
		return fmt.Sprintf("\"%s\"", inp)
	}
	return inp
}

const charsStr string = "0123456789aábcčdďeéěfghiíjklmnoópqrřsštťuůúvwxyý"
const asciStr string = "0123456789abcdefghijklmnopqrstuvwxy"

var charsArr []rune
var asciArr []rune

var randomStringInited = false

func generateRandomCode(codebook map[string][]string, fieldName string) string {
	fmt.Printf("")
	fieldNameUp := strings.ToUpper(fieldName)
	codes := codebook[fieldNameUp]
	if codes == nil {
		fmt.Printf("Codes are nil for %s \n", fieldNameUp)
		return "XXX"
	}
	return codes[rand.Intn(len(codes))]
}

func generateRandomStringInit() {
	if randomStringInited {
		return
	}
	lngt := utf8.RuneCount([]byte(charsStr))
	charsArr = make([]rune, 0, lngt)
	for _, r := range charsStr {
		charsArr = append(charsArr, r)
	}

	lngt = utf8.RuneCount([]byte(asciStr))
	asciArr = make([]rune, 0, lngt)
	for _, r := range asciStr {
		asciArr = append(asciArr, r)
	}
}

func generateRandomAsci(lng int) string {
	generateRandomStringInit()
	return generateRandomStringInternal(lng, asciArr)
}

func generateRandomString(lng int) string {
	generateRandomStringInit()
	return generateRandomStringInternal(lng, charsArr)
}

func generateRandomStringInternal(lngt int, charset []rune) string {
	w := make([]rune, 0, lngt)
	for i := 0; i < lngt; i++ {
		w = append(w, charset[rand.Intn(len(charset))])
	}
	return string(w)
}

func generateRandomDate(start string, end string) time.Time {
	startTime, _ := time.Parse("2006-01-02", start)
	endTime, _ := time.Parse("2006-01-02", end)
	startUnix := startTime.UnixNano()
	endUnix := endTime.UnixNano()
	diff := endUnix - startUnix
	return time.Unix(0, rand.Int63n(diff)+startUnix)
}

func generateFieldValue(fld Field) string {

	if fld.Mandatory == MandatoryIf || fld.Mandatory == NonMandatory {
		if rand.Intn(100) > 30 {
			return ""
		}
	}

	switch strings.ToUpper(fld.DomainType) {
	case "D_AMOUNT":
		return strconv.Itoa(rand.Intn(100000000) - 100000000)
	case "D_BINARY":
		if rand.Intn(100) > 50 {
			return "0"
		}
		return "1"
	case "D_COURSE":
		return strings.Replace(strconv.FormatFloat(float64(rand.Float32()*20.0), 'f', 3, 32), ".", ",", -1)
	case "D_DATE":
		return generateRandomDate("1990-01-01", "2021-12-31").Format("2006-01-02")
	case "D_DATETIME":
		return generateRandomDate("1990-01-01", "2021-12-31").Format("2006-01-02T15:04:05.6Z07:00")
	case "D_DECIMAL":
		return strings.Replace(strconv.FormatFloat(float64(rand.Float32()*20.0), 'f', 2, 32), ".", ",", -1)
	case "D_GPS":
		return strings.Replace(strconv.FormatFloat(float64(rand.Float32()*20.0), 'f', 7, 32), ".", ",", -1)
	case "D_IDENTIFIER":
		return generateRandomAsci(rand.Intn(20) + 3)
	case "D_INTEGER":
		return strconv.Itoa(rand.Intn(10000))
	case "D_KOD":
		return generateRandomCode(codesByField, fld.FieldName)
	case "D_MONEY":
		return strings.Replace(strconv.FormatFloat(float64(rand.Float32()*20.0), 'f', 2, 32), ".", ",", -1)
	case "D_REFERENCE":
		return generateRandomAsci(rand.Intn(20) + 3)
	case "D_TEXT":
		return generateRandomString(rand.Intn(50) + 3)
	}
	return generateRandomString(rand.Intn(20) + 3)
}

func loadCodebooks(codebooksCsv string) (codesByField map[string][]string, err error) {
	err = nil
	codesByField = make(map[string][]string)
	input, _ := os.Open(codebooksCsv)
	csvRdr := csv.NewReader(input)
	csvRdr.Comma = ','
	lineNo := 0
	hdr, _ := csvRdr.Read()
	fieldCol, _ := fldIdxByName("FieldName", hdr)
	valueCol, _ := fldIdxByName("Value", hdr)
	lineNo++
	for {
		rec, errRd := csvRdr.Read()
		if errRd == io.EOF {
			break
		}
		fldName := rec[fieldCol]
		val := rec[valueCol]

		fldNameUp := strings.ToUpper(fldName)
		if codesByField[fldNameUp] == nil {
			codesByField[fldNameUp] = make([]string, 0, 5)
		}
		codesByField[fldNameUp] = append(codesByField[fldNameUp], val)
	}

	return
}

func loadModelGameFile(modelGameFile string) (filesByModelGame map[string][]string, err error) {
	err = nil
	filesByModelGame = make(map[string][]string)
	input, _ := os.Open(modelGameFile)
	csvRdr := csv.NewReader(input)
	csvRdr.Comma = ','
	lineNo := 0
	hdr, _ := csvRdr.Read()
	modelCol, _ := fldIdxByName("Model", hdr)
	fileNameCol, _ := fldIdxByName("FileName", hdr)
	gameTypeCol, _ := fldIdxByName("GameType", hdr)
	lineNo++

	for {
		rec, err := csvRdr.Read()
		if err == io.EOF {
			break
		}
		model := strings.ToLower(rec[modelCol])
		gameType := strings.ToLower(rec[gameTypeCol])
		fileName := strings.ToLower(rec[fileNameCol])

		modelGame := fmt.Sprintf("%s-%s", model, gameType)
		if filesByModelGame[modelGame] == nil {
			filesByModelGame[modelGame] = make([]string, 0, 5)
		}
		filesByModelGame[modelGame] = append(filesByModelGame[modelGame], fileName)
	}
	return filesByModelGame, nil
}

func loadFields(csvSchema string) (filesByModel map[string][]string, fieldsByFileModel map[string][]Field, err error) {
	err = nil
	filesByModel = make(map[string][]string)
	fieldsByFileModel = make(map[string][]Field)
	input, _ := os.Open(csvSchema)
	csvRdr := csv.NewReader(input)
	csvRdr.Comma = ','
	lineNo := 0
	hdr, _ := csvRdr.Read()
	modelCol, _ := fldIdxByName("Model", hdr)
	fileNameCol, _ := fldIdxByName("FileName", hdr)
	fieldNameCzCol, _ := fldIdxByName("FieldNameCZ", hdr)
	domainTypeCol, _ := fldIdxByName("DomainType", hdr)
	minLengthCol, _ := fldIdxByName("MinLength", hdr)
	maxLengthCol, _ := fldIdxByName("MaxLength", hdr)
	presenceCol, _ := fldIdxByName("Presence", hdr)
	contextCol, _ := fldIdxByName("Context", hdr)
	lineNo++
	fields := make([]Field, 200)
	for {
		rec, err := csvRdr.Read()
		if err == io.EOF {
			break
		}
		minLen, _ := strconv.Atoi(rec[minLengthCol])
		maxLen, _ := strconv.Atoi(rec[maxLengthCol])
		fld := Field{
			Model:         strings.ToLower(rec[modelCol]),
			FileName:      strings.ToLower(rec[fileNameCol]),
			FieldName:     strings.ToLower(rec[fieldNameCzCol]),
			DefinedOnLine: lineNo,
			DomainType:    strings.ToLower(rec[domainTypeCol]),
			MinLen:        minLen,
			MaxLen:        maxLen,
			Mandatory:     parseMandatory(rec[presenceCol]),
			Context:       rec[contextCol],
		}
		fields = append(fields, fld)

		if filesByModel[fld.Model] == nil {
			filesByModel[fld.Model] = make([]string, 0, 20)
		}
		if fileNotPresent(filesByModel[fld.Model], fld.FileName) {
			filesByModel[fld.Model] = append(filesByModel[fld.Model], fld.FileName)
		}

		fileModel := fmt.Sprintf("%s-%s", fld.Model, fld.FileName)
		if fieldsByFileModel[fileModel] == nil {
			fieldsByFileModel[fileModel] = make([]Field, 0, 5)
		}
		fieldsByFileModel[fileModel] = append(fieldsByFileModel[fileModel], fld)
	}
	return
}
