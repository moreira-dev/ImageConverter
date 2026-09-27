# Image Converter

A command-line tool for converting images between JPG, PNG and WebP.

Image Converter is a simple .NET CLI project to practice good OOP principles.

![Menu](docs/Menu.png)

It is a first attempt at creating a useful CLI tool with .NET. It will serve as a foundation for a more advance project in the future.

The program places special focus on extensibility, allowing developers to add new menu options and image formats without modifying the existing code. 

To add a new image format see [Adding a new image format](#adding-a-new-image-format).
To add a new menu option see [Adding a new menu option](#adding-a-new-menu-option).

## Technology

* [.NET 10](https://learn.microsoft.com/dotnet/) with C#
* [ImageSharp](https://docs.sixlabors.com/articles/imagesharp/) to read and convert the images
* [Spectre.Console](https://spectreconsole.net/) for the CLI menus

## Features

* **Convert one image.** Choose an image file and choose the output format. A new file is saved next to the original image.
* **Convert all images in a folder.** Choose a folder and the output format. All images in that folder will be converted but images already in the output format are skipped.
* **Show image stats.** Choose a folder to see stats of all images within it.

## Local development

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download).

To run the app:

```
dotnet run
```

To create a build:

```
dotnet build
```

The output is written to `bin/`.

### Adding a new image format

Each format is a subclass of [FormatConverter](Models/Converters/FormatConverter.cs).

To add a format:

1. Add the format to [ImageFormat](Enums/ImageFormat.cs).
2. Create a new converter in `Models/Converters/` that inherits from [FormatConverter](Models/Converters/FormatConverter.cs).
3. Add the converter to the `supportedFormats` list in [Program.cs](Program.cs).

### Adding a new menu option

Each menu option is a class that implements [IMenuOption](Cli/Options/IMenuOption.cs).

To add a menu option:

1. Create a new class in `Cli/Options/` that implements [IMenuOption](Cli/Options/IMenuOption.cs).
2. Set `DisplayText` to the text shown in the menu, and put the option's logic in `Run()`.
3. Register the class in [Program.cs](Program.cs) with `services.AddSingleton<IMenuOption, YourOption>()`.

Options appear in the menu in the order they are registered, so add yours before `MenuExit`.

## Limitations and opportunities for improvement

* There are no config files or ways to customise the output
* Converting a folder does not include its subfolders, and it's not clear to the user if it will
* The user should be able to choose compression level

## License

See [LICENSE](LICENSE).
