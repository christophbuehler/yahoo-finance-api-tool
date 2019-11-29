# Yahoo Finance API Tool

Download market data from Yahoo Finance.

## Manual Mode

![Yahoo Finance API Tool](image.png)

## Bulk Mode

![Yahoo Finance API Tool](image_bulk.png)

### Example config file:
```
{
    "DateFrom": "29.04.2009",
    "DateUntil": "27.06.2017",
    "FileNaming": "{2}.csv",
    "Dir": "Output",
	"DecimalSeparator": ",",
    "Symbols": [
        "^DJI",
        "UVXY"
    ]
}
```