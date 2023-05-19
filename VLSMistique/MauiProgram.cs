/*
 *    Copyright 2023 Mathias Lund-Hansen
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using Microsoft.Maui.LifecycleEvents;
#if WINDOWS10_0_17763_0_OR_GREATER
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using Microsoft.UI.Xaml;
using WinRT;
using VLSMistique.Platforms.Windows;
#endif

namespace VLSMistique;

/// <summary> The main entry point for creating a Maui application. </summary>
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        // Create a builder for configuring the Maui app.
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>() // Set the startup App for Maui.
            .UseMauiCommunityToolkit() // Initialize the .NET MAUI Community Toolkit.

            // Configure custom fonts.
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .Services.AddSingleton<IFileSaver>(FileSaver.Default); // Register the default file saver service.

        // Add support to use the WinUI library.
        builder.ConfigureLifecycleEvents(events =>
        {
            #if WINDOWS10_0_17763_0_OR_GREATER
            events.AddWindows(wndLifeCycleBuilder =>
            {
                wndLifeCycleBuilder.OnWindowCreated(window =>
                {
                    window.TryMicaOrAcrylic(); //Adds the Mica or Acrylic effect depending on if it's Windows 10 or 11.
                });
            });
            #endif
        });

        return builder.Build(); // Build and return the configured Maui app.
    }
}