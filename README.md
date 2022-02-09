# File Cabinet
#### Description
Console application, which gets commands from user and manages user data.
## FileCabinetApp
### Command line options for FileCabinetApp

#### Data storaging
    --storage=type
    -s type

Types:
- memory
- file

#### Validation

    --validationrules=type
    -v type

Types:
   - custom
   - default

#### Service meter
    --use-stopwatch

#### Service logger
    --use-logger
    
### Commands for application

| Command | Parameters | Info|
| ------ | ------ | ----------|
|Create||Creates a new record|
|Help| <command> | Prints the help screen|
|Update| set [updating properties] where [searching properties]| updates specified properties in found records by some property|
|Delete| Where [searching property]| Deletes record|
|Purge||Defragmentates data file|
|Select | [selecting properties] (where [searching properties])|Prints selected properties of records, oprtional fount by concrete properties|
|Insert| ([property names]) values ([property values]) | Inserts record
|Export| [format] [file path]| Exports service data into file .csv or .xml|
|Import| [format] [file path] | Imports servcie data from file .csv or .xml|
|stat| | Prints the count of records|
|exit| | Exits the application|

### Examples

#### Import

```sh
> import csv d:\data\records.csv
10000 records were imported from d:\data\records.csv.

> import csv d:\data\records2.csv
Import error: file d:\data\records.csv is not exist.

> import xml c:\users\myuser\records.xml
5000 records were imported from c:\users\myuser\records.xml.
```
#### Export

```sh
> export csv filename.csv
All records are exported to file filename.csv.
> export csv e:\filename.csv
File is exist - rewrite e:\filename.csv? [Y/n] Y
All records are exported to file filename.csv.
> export xml e:\folder-not-exists\filename.xml
Export failed: can't open file e:\filename.csv.
```
#### Select

```
> select id, firstname, lastname where firstname = 'John' and lastname = 'Doe'
+----+-----------+----------+
| Id | FirstName | LastName |
+----+-----------+----------+
|  1 | John      | Doe      |
+----+-----------+----------+
> select lastname where firstname = 'John' or lastname = 'Smith'
+----------+
| LastName |
+----------+
| Doe      |
| Smith    |
+----------+
```
#### Insert
```
> insert (id, firstname, lastname, dateofbirth, sex, height, salary) values ('1', 'John', 'Doe', '5/18/1986', 'f', '190', '5000')
> insert (dateofbirth,lastname,firstname,id, salary, height, sex) values ('5/18/1986','Stan','Smith','2', '600', '200', 'M')
```

#### Delete

```
> delete where id = '1'
Record #1 is deleted.
> delete where LastName='Smith'
Records #2, #3, #4 are deleted. 
```


#### Update

```
> update set firstname = 'John', lastname = 'Doe' , dateofbirth = '5/18/1986' where id = '1'
> update set DateOfBirth = '5/18/1986' where FirstName='Stan' and LastName='Smith'
```


## FileCabinetGenerator
### Command line options

| Full Parameter Name | Short Parameter Name | Description                    |
|---------------------|----------------------|--------------------------------|
| output-type         | t                    | Output format type (csv, xml). |
| output              | o                    | Output file name.              |
| records-amount      | a                    | Amount of generated records.   |
| start-id            | i                    | ID value to start.             |

```sh
$ FileCabinetGenerator.exe --output-type=csv --output=d:\data\records.csv --records-amount=10000 --start-id=30
10000 records were written to records.csv.

$ FileCabinetGenerator.exe -t xml -o c:\users\myuser\records.xml -a 5000 -i 45
5000 records were written to c:\users\myuser\records.xml
```