# External String Sort
Внешняя сортировка строк для больших файлов (>1gb).
  
Сортирует строки по следующим правилам:  
1. Строка должна состоять из цифры, и текста, в формате **"{Цифра}. {Текст}"**;
2. Сортировка по возрастанию:  сначала сравнивается текстовая чать, если она совпадает у строк, сравнивается цифра;
3. Строки не уникальны и могут повторяться;
  
Утилита принимает параметры из командной строки, их описание можно прочитать в [файлах описания параметров](https://github.com/uu3474/ess/tree/master/src/ess/Verbs). Сортировка происходит многопоточно, то есть, в несколько потоков сортируются чанки и в несколько потоков происходит слияние. По умолчанию размер чанка равен 100 МБ, для сортировки используется число потоков, равное числу ядер процессора. Размер чанка можно настроить параметром **-с**, число потоков **-s**.
  
#### Примеры:  
Справка по командам
```
./ess --help
```
Справка по определенной команде, в данном примере по команде генерации тестовых данных
```
./ess gen --help
```
Cгенерировать тестовый файл размером 10 гигабайт
```
./ess gen -s 10g
```
Отсортировать тестовый файл (по умолчания будет взят сгенерированный файл) с параметрами по умолчанию
```
./ess sort
```
Отсортировать тестовый файл, с явно заданным именем входного и выходного файла
```
./ess sort -i input.txt -o output.txt
```
Отсортировать тестовый файл с размером чанка в 300 мегабайт, ограничив число потоков сортировки до 5
```
./ess sort -с 300m -s 5
```
  
#### Пример запуска
Пример сортировки файла размером 10 гигабайт, средняя длина строки 100 символов.  
На процессоре Ryzen 5 3600, SSD.  
  
![Screenshot](https://raw.githubusercontent.com/uu3474/ess/master/img/Screenshot%202020-09-12%20231155.png)
