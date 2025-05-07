import React from 'react';
import { View, Text, StyleSheet } from 'react-native';

const HomeMenu = () => {
  return (
    <View style={styles.container}>
      <Text style={styles.title}>Home Menu</Text>
      <Text style={styles.subtitle}>Navigate through our menu options.</Text>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#FFA500', // Màu cam
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    color: 'white',
  },
  subtitle: {
    fontSize: 16,
    color: 'white',
  },
});

export default HomeMenu; 