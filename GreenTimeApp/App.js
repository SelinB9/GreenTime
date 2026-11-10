import React from 'react';
import { StatusBar } from 'expo-status-bar';
import { StyleSheet, Text, View } from 'react-native';
import LoginScreen from './src/screen/LoginScreen';



export default function App() {
  return (
    <>
      <StatusBar style="dark" />
      <LoginScreen />
    </>
  );
}

